
namespace Matrix_App
{
    public static class Defaults
    {
        public const int PortNameUpdateInterval = 5000;

        public const int MatrixStartWidth = 16;
        public const int MatrixStartHeight = 16;
        public const int MatrixStartFrames = 1;

        public const int MatrixLimitedWidth = 512;
        public const int MatrixLimitedHeight = 512;

        public const int BaudRate = 48000;

        public const int ReadTimeoutMs = 5500;
        public const int WriteTimeoutMs = 5500;

        /// <summary>
        /// Total count of LEDs at start
        /// </summary>
        public static readonly int MATRIX_START_LED_COUNT = MatrixStartWidth * MatrixStartHeight * Bpp;

        /// <summary>
        ///  Number of Bytes Per Pixel: 3 cause Red (1 byte) + Blue (1 Byte) + Green (1 byte) = 3
        /// </summary>
        public const int Bpp = 3;

        public const int FilterPreviewWidth = 32;
        public const int FilterPreviewHeight = 32;

        public const int ArduinoSuccessByte = 21;

        public const int ArduinoCommandQueueSize = 2;
        public const int ArduinoReceiveBufferSize = 1 + 1 + 1 + MatrixLimitedWidth * MatrixLimitedHeight;

        public const int DequeueWaitTimeoutCounter = 3;
    }

    public static class ArduinoInstruction
    {
        public const byte OpcodeScale = 0;
        public const byte OpcodeImage = 2;
        public const byte OpcodeFill = 3;
        public static readonly byte OPCODE_CONFIG = 4;
    }
}