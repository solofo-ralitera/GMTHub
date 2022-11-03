#include <Servo.h>
#include "LedController.hpp" // https://github.com/noah1510/LedController

#define BOARD_NUMBER "1"
#define TOTAL_NUMBER_OF_PIN 20

//// DON'T EDIT BELOW /////////////////////////
#if defined(TEENSYDUINO) 
    //  --------------- Teensy -----------------
    #if defined(__AVR_ATmega32U4__)
        #define BOARD "Teensy 2.0"
    #elif defined(__AVR_AT90USB1286__)       
        #define BOARD "Teensy++ 2.0"
    #elif defined(__MK20DX128__)       
        #define BOARD "Teensy 3.0"
    #elif defined(__MK20DX256__)       
        #define BOARD "Teensy 3.2" // and Teensy 3.1 (obsolete)
    #elif defined(__MKL26Z64__)       
        #define BOARD "Teensy LC"
    #elif defined(__MK64FX512__)
        #define BOARD "Teensy 3.5"
    #elif defined(__MK66FX1M0__)
        #define BOARD "Teensy 3.6"
    #else
       #error "Unknown board"
    #endif
#else // --------------- Arduino ------------------
    #if   defined(ARDUINO_AVR_ADK)       
        #define BOARD "Mega Adk"
    #elif defined(ARDUINO_AVR_BT)    // Bluetooth
        #define BOARD "Bt"
    #elif defined(ARDUINO_AVR_DUEMILANOVE)       
        #define BOARD "Duemilanove"
    #elif defined(ARDUINO_AVR_ESPLORA)       
        #define BOARD "Esplora"
    #elif defined(ARDUINO_AVR_ETHERNET)       
        #define BOARD "Ethernet"
    #elif defined(ARDUINO_AVR_FIO)       
        #define BOARD "Fio"
    #elif defined(ARDUINO_AVR_GEMMA)
        #define BOARD "Gemma"
    #elif defined(ARDUINO_AVR_LEONARDO)       
        #define BOARD "Leonardo"
    #elif defined(ARDUINO_AVR_LILYPAD)
        #define BOARD "Lilypad"
    #elif defined(ARDUINO_AVR_LILYPAD_USB)
        #define BOARD "Lilypad Usb"
    #elif defined(ARDUINO_AVR_MEGA)       
        #define BOARD "Mega"
    #elif defined(ARDUINO_AVR_MEGA2560)       
        #define BOARD "Mega 2560"
    #elif defined(ARDUINO_AVR_MICRO)       
        #define BOARD "Micro"
    #elif defined(ARDUINO_AVR_MINI)       
        #define BOARD "Mini"
    #elif defined(ARDUINO_AVR_NANO)       
        #define BOARD "Nano"
    #elif defined(ARDUINO_AVR_NG)       
        #define BOARD "NG"
    #elif defined(ARDUINO_AVR_PRO)       
        #define BOARD "Pro"
    #elif defined(ARDUINO_AVR_ROBOT_CONTROL)       
        #define BOARD "Robot Ctrl"
    #elif defined(ARDUINO_AVR_ROBOT_MOTOR)       
        #define BOARD "Robot Motor"
    #elif defined(ARDUINO_AVR_UNO)       
        #define BOARD "Uno"
    #elif defined(ARDUINO_AVR_YUN)       
        #define BOARD "Yun"

    // These boards must be installed separately:
    #elif defined(ARDUINO_SAM_DUE)       
        #define BOARD "Due"
    #elif defined(ARDUINO_SAMD_ZERO)       
        #define BOARD "Zero"
    #elif defined(ARDUINO_ARC32_TOOLS)       
        #define BOARD "101"
    #else
       #error "Unknown board"
    #endif
#endif

// Serial port configuration
#define SERIAL_BAUD 9600
#define SERIAL_EOL '#'

// Scaning acknoledgement message
#define ACK_SCAN "ack_gmtscan_" BOARD "-" BOARD_NUMBER
#define ACK_READY "ready_" BOARD "-" BOARD_NUMBER

// Needed for Serial read
bool serialStringAvailable = false;
String serialString = "";

// TODO à optimiser la taille des tableaux à init
boolean arePinsInitialized[TOTAL_NUMBER_OF_PIN] = { false };
Servo availableServos[TOTAL_NUMBER_OF_PIN];
LedController<1,1> available7segMax7219[TOTAL_NUMBER_OF_PIN];

void setup() {
  Serial.begin(SERIAL_BAUD);

  // wait for serial port to connect. Needed for native USB port only
  while (!Serial) {
    ;
  }
}

/*
  SerialEvent occurs whenever a new data comes in the hardware serial RX. This
  routine is run between each time loop() runs, so using delay inside loop can
  delay response. Multiple bytes of data may be available.
*/
char inChar; // every incoming data from serial
void serialEvent() {
  while (Serial.available() > 0) {
    inChar = Serial.read();
    if ((char)inChar == SERIAL_EOL) {
      serialStringAvailable = true;
      break;
    }
    /*if (inChar == '\n' || inChar == '\r') {
      continue;
    }*/
    serialString += inChar;
  }
}

void resetSerial() {
  serialString = "";
  inChar = 0;
  serialStringAvailable = false;
}

void loop() {
  if(serialStringAvailable == false) {
    // No data to process
    return;
  }
  serialString.trim();
  if(serialString == "")  {
    // No data to process
    resetSerial();
    return;
  }

  if (serialString == "gmtscan") {
    // Send acknoledgement to C# program (when scaning available devices)
    Serial.println(ACK_SCAN);
    Serial.flush();
    resetSerial();
    return;
  }

  // Check si les données sont complètes
  if(!serialString.startsWith(":") && !serialString.endsWith(":")) {
    // No thing to do
    resetSerial();
    return;
  }
  
  // Suppr le delim : au début de la chaine
  // TODO remove le if après test
  if(serialString.startsWith(":")) {
    serialString.remove(0, 1);
  }

  // :s03050:#    Test servo
  // :d071:#    Test digital
  // :a05066:#  Test analog
  // :m100906081234:#   Test Max7219
  // :s03050d021:#
  // :s03052d070d040:#
  // 4 :length min des commandes
  while(serialString.length() > 4) {
    short pin = serialString.substring(1, 3).toInt();
    char code = serialString.charAt(0);

    // Digital output
    if(code == 'd') {
      if(arePinsInitialized[pin] == false) {
        pinMode(pin, OUTPUT);
        arePinsInitialized[pin] = true;
      }
      short state = serialString.substring(3, 4).toInt();
      digitalWrite(pin, state ? HIGH : LOW);
      serialString.remove(0, 4);
    }
    // Analog output
    else if(code == 'a') {
      if(arePinsInitialized[pin] == false) {
        pinMode(pin, OUTPUT);
        arePinsInitialized[pin] = true;
      }      
      short value = serialString.substring(3, 6).toInt();
      analogWrite(pin, value); 
      serialString.remove(0, 6);
    }
    // Servo
    else if(code == 's') {
      if(arePinsInitialized[pin] == false) {
        availableServos[pin].attach(pin);
        arePinsInitialized[pin] = true;
      }
      short angle = serialString.substring(3, 6).toInt();
      availableServos[pin].write(angle); 
      serialString.remove(0, 6);
    }
    // Max 7seg Display
    else if(code == 'm') {
      // m[2 PIN][2 DIN][2 CS][2 CLK][2 Digit length][8 number to display]
      // :m1616181704   0#:
      short pinDIN = serialString.substring(3, 5).toInt();
      short pinCS = serialString.substring(5, 7).toInt();
      short pinCLK = serialString.substring(7, 9).toInt();
      short displayOffset = serialString.substring(9, 11).toInt();
      short digitLen = serialString.substring(11, 13).toInt();
      // Use pinDIN as key to allow up to 3 configurations on a single 7seg
      if(arePinsInitialized[pinDIN] == false) {
        available7segMax7219[pinDIN] = LedController<1,1>(pinDIN, pinCLK, pinCS); // DIN,CLK,CS
        available7segMax7219[pinDIN].setIntensity(8);
        available7segMax7219[pinDIN].clearMatrix();
        arePinsInitialized[pinDIN] = true;
      }
      String numberToDisplay = serialString.substring(13, 13 + digitLen);
      for(int i=0; i < digitLen; i++) {
        available7segMax7219[pinDIN].setChar(0, displayOffset + i, numberToDisplay[i], false);
      }
      serialString.remove(0, 13 + digitLen);
    }
    else {
      // Fallback
      serialString.remove(0, 6);
    }

  }

  // End of Data
  resetSerial();
}
