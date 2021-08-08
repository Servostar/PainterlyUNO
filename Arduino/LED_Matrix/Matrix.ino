#include "Arduino.h"
#include "FastLED.h"

#define LED_TYPE WS2812B
#define DATA_PIN 6

#define MATRIX_MAX_WIDTH 20
#define MATRIX_MAX_HEIGHT 20
#define MATRIX_LED_MAX_COUNT (MATRIX_MAX_WIDTH * MATRIX_MAX_HEIGHT)

#define STD_WIDTH 16
#define STD_HEIGHT 16
#define STD_LED_MAX_COUNT (STD_WIDTH * STD_HEIGHT)

//#define DEBUG_PRINT_CALLBACK

#define WAIT while(!Serial.available());

uint8_t opcode = 99;

uint8_t width = STD_WIDTH;
uint8_t height = STD_HEIGHT;

uint32_t ledCount;

CRGB leds[MATRIX_LED_MAX_COUNT];

typedef void (*FNPTR_t)();

uint8_t getByte() {
	WAIT
	return Serial.read();
}

uint16_t getWord() {
	uint16_t highByte = getByte();
	uint16_t lowByte  = getByte();

	return highByte << 8 | lowByte;
}

void scale() {
#ifdef DEBUG_PRINT_CALLBACK
  Serial.println("scale called");
#endif
	width  = getByte();
	height = getByte();

#ifdef DEBUG_PRINT_CALLBACK
  Serial.print("Width: ");
  Serial.println(width);

  Serial.print("Height: ");
  Serial.println(height);
#endif

	uint16_t newLedCount = width * height;

	if (newLedCount <= MATRIX_LED_MAX_COUNT) {
		ledCount = newLedCount;
	} else {
		return;
	}

#ifdef DEBUG_PRINT_CALLBACK
  Serial.print("LEDs: ");
  Serial.println(ledCount);
#endif

	FastLED.addLeds<LED_TYPE, DATA_PIN, GRB>(leds, ledCount);

	for (uint16_t x = 0; x < ledCount; x++) {
		leds[x] = 0;
	}

	FastLED.show();
}

void single() {
#ifdef DEBUG_PRINT_CALLBACK
  Serial.println("Single called");
#endif
	uint16_t index = getWord();

	uint8_t green = getByte();
	uint8_t red   = getByte();
	uint8_t blue  = getByte();

#ifdef DEBUG_PRINT_CALLBACK
	Serial.print("Index: ");
	Serial.println(index);

  Serial.print("Red: ");
  Serial.println(red);

  Serial.print("Green: ");
  Serial.println(green);

  Serial.print("Blue: ");
  Serial.println(blue);
#endif

	leds[index] = CRGB(red, green, blue);

	FastLED.show();
}

void image() {

	Serial.readBytes((char*) leds, ledCount * 3);

	FastLED.show();
}

void fill() {
#ifdef DEBUG_PRINT_CALLBACK
  Serial.println("Called fill");
#endif

	uint8_t green = getByte();
	uint8_t red   = getByte();
	uint8_t blue  = getByte();

#ifdef DEBUG_PRINT_CALLBACK
  Serial.print("Red: ");
  Serial.println(red);

  Serial.print("Green: ");
  Serial.println(green);

  Serial.print("Blue: ");
  Serial.println(blue);
#endif

	for (uint16_t x = 0; x < ledCount; x++) {
		leds[x].r = red;
		leds[x].g = green;
		leds[x].b = blue;
	}

	FastLED.show();
}

void config() {
	Serial.write(width);
	Serial.write(height);

	for (uint32_t i = 0; i < ledCount; i++){

		Serial.write((uint8_t) leds[i].r);
		Serial.write((uint8_t) leds[i].g);
		Serial.write((uint8_t) leds[i].b);
	}
}

void upload() {
	return;
}

void info() {
	Serial.write((uint8_t) 91);
	Serial.write((uint8_t) 0b01000000);
	Serial.write("ATmega328P Arduino");
}

FNPTR_t opcodeTable[] = {
		scale,   // opcode 0x00
		single,  // opcode 0x01
		image,   // opcode 0x02
		fill,    // opcode 0x03
		config,	 // opcode 0x04
		upload,
		info
	};

void setup() {
	ledCount = STD_LED_MAX_COUNT;

	Serial.begin(9600);

	FastLED.addLeds<LED_TYPE, DATA_PIN, GRB>(leds, ledCount);
	FastLED.setCorrection(TypicalLEDStrip);
	FastLED.setBrightness(80);

	for (uint16_t i = 0; i < ledCount; i++) {
		leds[i] = 0;
	}

	FastLED.show();
}

void loop() {
	if (Serial.available()) {
#ifdef DEBUG_PRINT_CALLBACK
    Serial.println("Opcode read in");
#endif

		opcode = getByte();

#ifdef DEBUG_PRINT_CALLBACK
    Serial.print("Opcode changed to:");
    Serial.println(opcode);
#endif

		if (opcode <= 6) {
			opcodeTable[opcode]();
			Serial.write(75);
		}
	}
}
