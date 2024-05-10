using UnityEngine;
using System;
using System.IO.Ports;
using System.Runtime.InteropServices;

public class PicoProtocolV2 : MonoBehaviour
{
    private SerialPort serialPort;
    private static int DataPacketSize = Marshal.SizeOf<DataPacket>();

    // Struct to hold the received data
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct DataPacket
    {
        public bool temperature;
        public bool pressure;
        public bool dynamic;
        public short temperatureValue;
        public short pressureValue;
        public short dynamicValue;
    }

    private void Start()
    {
        // Configure the serial port
        string portName = "COM3"; // Replace with the appropriate port name
        int baudRate = 115200;
        serialPort = new SerialPort(portName, baudRate);
        serialPort.Open();
    }

    private void Update()
    {
        ReceiveDataPacket();
    }

    private void OnApplicationQuit()
    {
        serialPort.Close();
    }

    // Function to calculate the XOR checksum
    private byte CalculateChecksum(byte[] data)
    {
        byte checksum = 0;
        for (int i = 0; i < data.Length - 1; ++i)
        {
            checksum ^= data[i];
        }
        return checksum;
    }

    // Function to receive and parse the DataPacket
    private void ReceiveDataPacket()
    {
        if (serialPort.BytesToRead >= DataPacketSize + 1)
        {
            byte[] buffer = new byte[DataPacketSize + 1];
            serialPort.Read(buffer, 0, buffer.Length);

            // Extract the data and checksum from the buffer
            byte[] dataBytes = new byte[DataPacketSize];
            Array.Copy(buffer, dataBytes, DataPacketSize);
            byte receivedChecksum = buffer[buffer.Length - 1];

            // Calculate the checksum from the received data
            byte calculatedChecksum = CalculateChecksum(dataBytes);

            // Compare the checksums
            if (receivedChecksum == calculatedChecksum)
            {
                // Checksums match, parse the data
                DataPacket packet = BytesToStructure<DataPacket>(dataBytes);
                // Use the parsed data here
                Debug.Log($"Temperature: {packet.temperature}");
                Debug.Log($"Pressure: {packet.pressure}");
                Debug.Log($"Dynamic: {packet.dynamic}");
                Debug.Log($"Temperature Value: {packet.temperatureValue}");
                Debug.Log($"Pressure Value: {packet.pressureValue}");
                Debug.Log($"Dynamic Value: {packet.dynamicValue}");
            }
            else
            {
                // Checksums don't match, discard the data
                Debug.Log("Checksum mismatch, data corrupted.");
            }
        }
    }

    // Helper function to convert a byte array to a struct
    private T BytesToStructure<T>(byte[] bytes) where T : struct
    {
        GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        T stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
        handle.Free();
        return stuff;
    }
}
