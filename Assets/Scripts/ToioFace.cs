using UnityEngine;
using CoreBluetooth;

public class ToioFace : MonoBehaviour, ICBCentralManagerDelegate, ICBPeripheralDelegate
{
    string _serviceUUID = "0B21C05A-44C2-47CC-BFEF-4F7165C33908";
    string _expressionCharacteristicUUID = "B3C450C9-5FC5-48F6-9EFD-D588E494F462";

    CBCentralManager _centralManager;
    CBPeripheral _peripheral;
    CBCharacteristic _expressionCharacteristic;

    void Start()
    {
        var initOptions = new CBCentralManagerInitOptions() { ShowPowerAlert = true };
        _centralManager = new CBCentralManager(this, initOptions);
    }

    void ICBCentralManagerDelegate.DidUpdateState(CBCentralManager central)
    {
        if (central.State == CBManagerState.PoweredOn)
        {
            Debug.Log("Scanning...");
            central.ScanForPeripherals(new string[] { _serviceUUID });
        }
    }

    void ICBCentralManagerDelegate.DidDiscoverPeripheral(CBCentralManager central, CBPeripheral peripheral, int rssi)
    {
        _peripheral = peripheral;
        peripheral.Delegate = this;
        central.StopScan();
        central.Connect(peripheral);
    }

    void ICBCentralManagerDelegate.DidConnectPeripheral(CBCentralManager central, CBPeripheral peripheral)
    {
        peripheral.DiscoverServices(new string[] { _serviceUUID });
    }

    void ICBPeripheralDelegate.DidDiscoverServices(CBPeripheral peripheral, CBError error)
    {
        if (error != null)
        {
            Debug.LogError($"[DidDiscoverServices] error: {error}");
            return;
        }

        foreach (var service in peripheral.Services)
        {
            peripheral.DiscoverCharacteristics(new string[] { _expressionCharacteristicUUID }, service);
        }
    }

    void ICBPeripheralDelegate.DidDiscoverCharacteristics(CBPeripheral peripheral, CBService service, CBError error)
    {
        if (error != null)
        {
            Debug.LogError($"[DidDiscoverCharacteristics] error: {error}");
            return;
        }

        foreach (var characteristic in service.Characteristics)
        {
            if (characteristic.UUID == _expressionCharacteristicUUID)
            {
                _expressionCharacteristic = characteristic;
                Debug.Log("Connected");
                return;
            }
        }
    }

    void OnDestroy()
    {
        if (_centralManager != null)
        {
            _centralManager.Dispose();
            _centralManager = null;
        }
    }
}
