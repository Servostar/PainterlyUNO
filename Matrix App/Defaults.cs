
namespace Matrix_App
{
    public static class Defaults
    {
        public const int MatrixStartWidth = 16;
        public const int MatrixStartHeight = 16;
        public const int MatrixStartFrames = 1;

        public const int MatrixLimitedWidth = 512;
        public const int MatrixLimitedHeight = 512;

        public const int BaudRate = 9600;

        public const int ReadTimeoutMs = 20500;
        public const int WriteTimeoutMs = 2500;
        
        /// <summary>
        ///  Number of Bytes Per Pixel: 3 cause Red (1 byte) + Blue (1 Byte) + Green (1 byte) = 3
        /// </summary>
        public const int Bpp = 3;

        public const int FilterPreviewWidth = 32;
        public const int FilterPreviewHeight = 32;

        public const int ArduinoSynchronizationByte = 91;
        public const int ArduinoSuccessByte = 75;
        public const int ArduinoErrorByte = 255;

        public const int ArduinoCommandQueueSize = 2;
        public const int ArduinoReceiveBufferSize = 1 + 1 + 1 + MatrixLimitedWidth * MatrixLimitedHeight;

        public const int DequeueWaitTimeoutCounter = 3;
    }

    public static class ArduinoInstruction
    {
        public const byte OpcodeScale = 0;
        public const byte OpcodeImage = 2;
        public const byte OpcodeFill = 3;
        public const byte OpcodePush = 5;

        public const byte OpcodeInfo = 6;
//      public static readonly byte OpcodeConfig = 4;
    }
}