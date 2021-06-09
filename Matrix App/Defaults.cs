
namespace MatrixDesigner
{
    public sealed class Defaults
    {
        public static readonly int PORT_NAME_UPDATE_INTERVAL = 5000;

        public static readonly int MATRIX_START_WIDTH = 10;
        public static readonly int MATRIX_START_HEIGHT = 10;
        public static readonly int MATRIX_START_FRAMES = 1;

        public static readonly int MATRIX_LIMITED_WIDTH = 512;
        public static readonly int MATRIX_LIMITED_HEIGHT = 512;

        public static readonly int BAUD_RATE = 9600;

        public static readonly int READ_TIMEOUT_MS = 5500;
        public static readonly int WRITE_TIMEOUT_MS = 5500;

        /// <summary>
        /// Total count of LEDs at start
        /// </summary>
        public static readonly int MATRIX_START_LED_COUNT = MATRIX_START_WIDTH * MATRIX_START_HEIGHT * BPP;

        /// <summary>
        ///  Number of Bytes Per Pixel: 3 cause Red (1 byte) + Blue (1 Byte) + Green (1 byte) = 3
        /// </summary>
        public static readonly int BPP = 3;

        public static readonly int FILTER_PREVIEW_WIDTH  = 32;
        public static readonly int FILTER_PREVIEW_HEIGHT = 32;

        public static readonly int ARDUINO_SUCCESS_BYTE = 21;

        public static readonly int ARDUINO_COMMAND_QUEUE_SIZE  = 5;
        public static readonly int ARDUINO_RECIVCE_BUFFER_SIZE = 1 + 1 + 1 + MATRIX_LIMITED_WIDTH * MATRIX_LIMITED_HEIGHT;

        public static readonly int DEQUEUE_WAIT_TIMEOUT_COUNTER = 2;
    }

    public sealed class ArduinoInstruction
    {
        public static readonly byte OPCODE_SCALE  = 0;
//        public static readonly byte OPCODE_SINGLE = 1;
        public static readonly byte OPCODE_IMAGE  = 2;
        public static readonly byte OPCODE_FILL   = 3;
        public static readonly byte OPCODE_CONFIG = 4;
    }
}