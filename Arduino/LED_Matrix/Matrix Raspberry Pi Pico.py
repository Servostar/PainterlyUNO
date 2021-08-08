from time import sleep

from machine import UART, Pin

import array, time
from machine import Pin
import rp2

# Configure the number of WS2812 LEDs.
NUM_LEDS = 256
PIN_NUM = 16
brightness = 0.2

@rp2.asm_pio(sideset_init=rp2.PIO.OUT_LOW, out_shiftdir=rp2.PIO.SHIFT_LEFT, autopull=True, pull_thresh=24)
def ws2812():
    T1 = 2
    T2 = 5
    T3 = 3
    wrap_target()
    label("bitloop")
    out(x, 1)               .side(0)    [T3 - 1]
    jmp(not_x, "do_zero")   .side(1)    [T1 - 1]
    jmp("bitloop")          .side(1)    [T2 - 1]
    label("do_zero")
    nop()                   .side(0)    [T2 - 1]
    wrap()

# Create the StateMachine with the ws2812 program, outputting on pin
sm = rp2.StateMachine(0, ws2812, freq=8_000_000, sideset_base=Pin(PIN_NUM))

# Start the StateMachine, it will wait for data on its FIFO.
sm.active(1)

# Display a pattern on the LEDs via an array of LED RGB values.
global ar
global leds
global gif
global currentFrame
global delay

##########################################################################

def show():
    for i in range(leds):
        r = ar[i] >> 16
        g = ar[i] >> 8 & 0xFF
        b = ar[i] & 0xFF
        
        r = r >> 1
        g = g >> 1
        b = b >> 1
        
        ar[i] = r << 16 | g << 8 | b
        
    sm.put(ar, 8)

leds = 256              # number of leds
uart = UART(0, 9600)    # serial bluetooth
led = Pin(25, Pin.OUT)  # builtin LED
ar = array.array("I", [0 for _ in range(NUM_LEDS)]) # color array
gif = [array.array("I", [0 for _ in range(leds)]) for _ in range(1)]
currentFrame = -1

# gurantees that only N-bytes are read from the UART
def readNBytes(n):
    rawBytes = b''
    bytesRead = 0
    while True:
        if uart.any():
            rawBytes += uart.read(n - bytesRead)
   
        bytesRead = len(rawBytes)
        
        # not enough was read
        if bytesRead < n:
            continue
        else:
            return rawBytes

while True:
    led.low()
    if uart.any() != 0:
        opcode = uart.read(1)
        
        print("Opcode: ", opcode)

        if opcode == b'\x00':
            width  = readNBytes(1)[0]
            height = readNBytes(1)[0]

            if width <= 32 and height <= 32:
                leds = width * height
            
                ar = array.array("I", [0 for _ in range(leds)])
                show()

        elif opcode == b'\x02':   
            rawBytes = readNBytes(leds * 3);
                
            for i in range(leds):
                ar[i] = rawBytes[i * 3 + 1] << 16 | rawBytes[i * 3] << 8 | rawBytes[i * 3 + 2]
       
            show()
            
            currentFrame = -1
            
        elif opcode == b'\x03':
            r = readNBytes(1)
            g = readNBytes(1)
            b = readNBytes(1)

            for i in range(leds):
                ar[i] = g[0] << 16 | r[0] << 8 | b[0]
       
            show()
            
        elif opcode == b'\x04':
            
            uart.write([75])
        
        # [width] [height] [frames] [delay] [rgb-frames]
        elif opcode == b'\x05':
            gifDim = readNBytes(5)
            
            delay = (gifDim[3] << 8) | gifDim[4]
            
            print("w ", gifDim[0], " h ", gifDim[1], " f ", gifDim[2], " d ", delay)
            
            leds = gifDim[0] * gifDim[1]
            gif = [array.array("I", [0 for _ in range(leds)]) for _ in range(gifDim[2])] 
            
            for f in range(gifDim[2]):
                frame = readNBytes(leds * 3)
                print("frame read: ", f)
                for i in range(leds):
                    gif[f][i] = frame[i * 3 + 1] << 16 | frame[i * 3] << 8 | frame[i * 3 + 2]
                #uart.write(bytearray([91])) # synchronize
                 
            currentFrame = 0
            
            print("Everything read")
        
        # [Synchro-byte] [feature-flags] [Controller-Id]
        #
        # feature-flags:
        #   [Bit 7] = Bluetooth (true)
        #   [Bit 6] = USB (false)
        #   [Bit 0] = Upload (true)
        #
        elif opcode == b'\x06':
            uart.write(b'\x5B\x81')
            uart.write("RP2040 Micro python")

        led.high()
        uart.write(bytearray([75]))
    
    elif currentFrame != -1:
        print("showing")
        for i in range(leds):
            ar[i] = gif[currentFrame][i]
            
        show()
        currentFrame += 1
        
        if (currentFrame >= len(gif)):
            currentFrame = 0
            
        time.sleep(delay * 1e-3)

