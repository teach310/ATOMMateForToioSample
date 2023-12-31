using UnityEngine;
using CoreBluetooth;
using System;

public class ToioFace : MonoBehaviour, ICBCentralManagerDelegate, ICBPeripheralDelegate
{
    public enum Expression
    {
        Happy = 0,
        Angry = 1,
        Sad = 2,
        Doubt = 3,
        Sleepy = 4,
        Neutral = 5
    }

    string _serviceUUID = "0B21C05A-44C2-47CC-BFEF-4F7165C33908";
    string _expressionCharacteristicUUID = "B3C450C9-5FC5-48F6-9EFD-D588E494F462";
    string _distanceCharacteristicUUID = "29C2D1B2-944A-4FBA-AFCD-133E09532556";

    CBCentralManager _centralManager;
    CBPeripheral _peripheral;
    CBCharacteristic _expressionCharacteristic;
    CBCharacteristic _distanceCharacteristic;

    bool _connectedToPeripheral = false;

    public int Distance { get; set; } = 0; // [mm]
    public int LatestDistanceUpdateFrame { get; set; } = 0;

    public bool Connected
    {
        get
        {
            if (!_connectedToPeripheral) return false;
            return _expressionCharacteristic != null;
        }
    }

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
        _connectedToPeripheral = true;
        peripheral.DiscoverServices(new string[] { _serviceUUID });
    }

    void ICBCentralManagerDelegate.DidDisconnectPeripheral(CBCentralManager central, CBPeripheral peripheral, CBError error)
    {
        _connectedToPeripheral = false;
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
            peripheral.DiscoverCharacteristics(new string[] { _expressionCharacteristicUUID, _distanceCharacteristicUUID }, service);
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
            }

            if (characteristic.UUID == _distanceCharacteristicUUID)
            {
                _distanceCharacteristic = characteristic;
                peripheral.SetNotifyValue(true, characteristic);
            }
        }
    }

    void ICBPeripheralDelegate.DidUpdateValueForCharacteristic(CBPeripheral peripheral, CBCharacteristic characteristic, CBError error)
    {
        if (error != null)
        {
            Debug.LogError($"[DidUpdateValueForCharacteristic] error: {error}");
            return;
        }

        if (characteristic.UUID == _distanceCharacteristicUUID)
        {
            var data = characteristic.Value;
            int distance = BitConverter.ToInt16(data, 0);
            // Debug.Log($"Distance: {distance}");
            Distance = distance;
            LatestDistanceUpdateFrame = Time.frameCount;
        }
    }

    public void SetExpression(Expression expression)
    {
        if (_expressionCharacteristic == null)
        {
            Debug.LogError("Not connected");
            return;
        }

        var data = new byte[1];
        data[0] = BitConverter.GetBytes((int)expression)[0];
        _peripheral.WriteValue(data, _expressionCharacteristic, CBCharacteristicWriteType.WithResponse);
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
