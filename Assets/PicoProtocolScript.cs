using UnityEngine;
using System.IO.Ports;
using TMPro;
using System;
using System.Runtime.InteropServices;


public class PICOPROTOCOL : MonoBehaviour
{
    // Variables
    private SerialPort SerialP;
    public TextMeshProUGUI VariableA;
    public TextMeshProUGUI VariableB;
    public TextMeshProUGUI Temperatura;
    public bool TemperaturaStatus;
    public bool PresionStatus;
    public bool HumedadStatus;
    bool temperature;
    bool pressure;
    bool humidity;
    Int16 TempValue;
    Int16 PresValue;
    Int16 HumValue;

    // Define the data packet that will be used as Struct container for all data
    [StructLayout(LayoutKind.Sequential)]
    public struct DataPacket
    {
        public bool temperature;
        public bool pressure;
        public bool dynamic;
        public Int16 temperatureValue;
        public Int16 pressureValue;
        public Int16 dynamicValue;
    }

    void ParseDataPacket(byte[] buffer) //Funcion in order to recieve the unorganized data, organize it and be able to acces it
    {
        // Create a handle to pin the buffer and access it as an unmanaged memory block
        GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        IntPtr pointer = handle.AddrOfPinnedObject();

        // Marshal the unmanaged memory block into a managed DataPacket struct
        DataPacket packet = Marshal.PtrToStructure<DataPacket>(pointer);

        // Access the individual members of the DataPacket
        temperature = packet.temperature;
        pressure = packet.pressure;
        humidity = packet.dynamic;
        TempValue = packet.temperatureValue;
        PresValue = packet.pressureValue;
        HumValue = packet.dynamicValue;

        // Free the handle after using it
        handle.Free();
    }

    void Start() //En el start del programa inicializa la comunicacion serial con un puerto y un baud rate
    {
        SerialP = new SerialPort();
        SerialP.PortName = "COM9";
        SerialP.BaudRate = 115200;
        SerialP.DtrEnable = true;
        SerialP.Open();
        Debug.Log("Open Serial Port");

    }

    void Update() // Si el puerto serial si se abrio y hay mas de o exactamente 4 bytes para leer va a crear byte data va a leer lo que haya en el serial y lo graba en byte
    {
        if (Input.GetKeyDown(KeyCode.A)) ///Send number 1
        {
            byte[] data = { 0x31 };// start
            SerialP.Write(data, 0, 1);
            Debug.Log("Sent 1 to pico");
        }
        if (Input.GetKeyDown(KeyCode.B)) ///Send number 2
        {
            byte[] data = { 0x32 };// start
            SerialP.Write(data, 0, 1);
            Debug.Log("Sent 2 to pico");
        }
        if (Input.GetKeyDown(KeyCode.C)) ///Send number 3
        {
            byte[] data = { 0x33 };// start
            SerialP.Write(data, 0, 1);
            Debug.Log("Sent 1 to pico");
        }
        if (Input.GetKeyDown(KeyCode.D)) ///Send number 4
        {
            byte[] data = { 0x34 };// start
            SerialP.Write(data, 0, 1);
            Debug.Log("Sent 4 to pico");
        }

        if (SerialP.IsOpen && SerialP.BytesToRead >= 4) 
        {

            byte[] dataPacket = new byte[4];
            SerialP.Read(dataPacket, 0, 4);

            ParseDataPacket(dataPacket);

            Temperatura.text = TempValue.ToString();
            VariableA.text = PresValue.ToString();
            VariableB.text = HumValue.ToString();

            TemperaturaStatus = temperature;
            PresionStatus = pressure;
            HumedadStatus = humidity;

        }
    }





}


