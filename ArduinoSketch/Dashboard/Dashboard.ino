#include <Servo.h>
#include "LedController.hpp" // https://github.com/noah1510/LedController
// #include <LiquidCrystal_PCF8574.h> // https://github.com/mathertel/LiquidCrystal_PCF8574/

#define BOARD_NUMBER "1"

#define LCD_ADRESS 0x27
#define LCD_COLUMN 20
#define LCD_ROW 4


//// DON'T EDIT BELOW /////////////////////////
#define TOTAL_NUMBER_OF_PIN 20
#if defined(TEENSYDUINO) 
    //  --------------- Teensy -----------------
    #if defined(__AVR_ATmega32U4__)
        #define BOARD "Teensy 2.0"
        #define TOTAL_NUMBER_OF_PIN 21
    #elif defined(__AVR_AT90USB1286__)       
        #define BOARD "Teensy++ 2.0"
        #define TOTAL_NUMBER_OF_PIN 27
    #elif defined(__MK20DX128__)       
        #define BOARD "Teensy 3.0"
        #define TOTAL_NUMBER_OF_PIN 23
    #elif defined(__MK20DX256__)       
        #define BOARD "Teensy 3.2" // and Teensy 3.1 (obsolete)
        #define TOTAL_NUMBER_OF_PIN 23
    #elif defined(__MKL26Z64__)       
        #define BOARD "Teensy LC"
        #define TOTAL_NUMBER_OF_PIN 23
    #elif defined(__MK64FX512__)
        #define BOARD "Teensy 3.5"
        #define TOTAL_NUMBER_OF_PIN 39
    #elif defined(__MK66FX1M0__)
        #define BOARD "Teensy 3.6"
        #define TOTAL_NUMBER_OF_PIN 39
    #else
       #error "Unknown board"
    #endif
#else // --------------- Arduino ------------------
    #if   defined(ARDUINO_AVR_ADK)       
        #define BOARD "Mega Adk"
        #define TOTAL_NUMBER_OF_PIN 70
    #elif defined(ARDUINO_AVR_BT)    // Bluetooth
        #define BOARD "Bt"
        #define TOTAL_NUMBER_OF_PIN 1
    #elif defined(ARDUINO_AVR_DUEMILANOVE)       
        #define BOARD "Duemilanove"
        #define TOTAL_NUMBER_OF_PIN 19
    #elif defined(ARDUINO_AVR_ESPLORA)       
        #define BOARD "Esplora"
        #define TOTAL_NUMBER_OF_PIN 1
    #elif defined(ARDUINO_AVR_ETHERNET)       
        #define BOARD "Ethernet"
        #define TOTAL_NUMBER_OF_PIN 1
    #elif defined(ARDUINO_AVR_FIO)       
        #define BOARD "Fio"
        #define TOTAL_NUMBER_OF_PIN 21
    #elif defined(ARDUINO_AVR_GEMMA)
        #define BOARD "Gemma"
        #define TOTAL_NUMBER_OF_PIN 1
    #elif defined(ARDUINO_AVR_LEONARDO)       
        #define BOARD "Leonardo"
        #define TOTAL_NUMBER_OF_PIN 19
    #elif defined(ARDUINO_AVR_LILYPAD)
        #define BOARD "Lilypad"
        #define TOTAL_NUMBER_OF_PIN 1
   #elif defined(ARDUINO_AVR_LILYPAD_USB)
        #define BOARD "Lilypad Usb"
        #define TOTAL_NUMBER_OF_PIN 1
    #elif defined(ARDUINO_AVR_MEGA)       
        #define BOARD "Mega"
        #define TOTAL_NUMBER_OF_PIN 70
    #elif defined(ARDUINO_AVR_MEGA2560)       
        #define BOARD "Mega 2560"
        #define TOTAL_NUMBER_OF_PIN 70
    #elif defined(ARDUINO_AVR_MICRO)       
        #define BOARD "Micro"
        #define TOTAL_NUMBER_OF_PIN 19
    #elif defined(ARDUINO_AVR_MINI)       
        #define BOARD "Mini"
        #define TOTAL_NUMBER_OF_PIN 17
    #elif defined(ARDUINO_AVR_NANO)       
        #define BOARD "Nano"
        #define TOTAL_NUMBER_OF_PIN 21
    #elif defined(ARDUINO_AVR_NG)       
        #define BOARD "NG"
        #define TOTAL_NUMBER_OF_PIN 19
    #elif defined(ARDUINO_AVR_PRO)       
        #define BOARD "Pro"
        #define TOTAL_NUMBER_OF_PIN 17
    #elif defined(ARDUINO_AVR_ROBOT_CONTROL)       
        #define BOARD "Robot Ctrl"
        #define TOTAL_NUMBER_OF_PIN 1
    #elif defined(ARDUINO_AVR_ROBOT_MOTOR)       
        #define BOARD "Robot Motor"
        #define TOTAL_NUMBER_OF_PIN 1
    #elif defined(ARDUINO_AVR_UNO)       
        #define BOARD "Uno"
        #define TOTAL_NUMBER_OF_PIN 19
    #elif defined(ARDUINO_AVR_YUN)       
        #define BOARD "Yun"
        #define TOTAL_NUMBER_OF_PIN 19

    // These boards must be installed separately:
    #elif defined(ARDUINO_SAM_DUE)       
        #define BOARD "Due"
        #define TOTAL_NUMBER_OF_PIN 66
    #elif defined(ARDUINO_SAMD_ZERO)       
        #define BOARD "Zero"
        #define TOTAL_NUMBER_OF_PIN 19
    #elif defined(ARDUINO_ARC32_TOOLS)       
        #define BOARD "101"
        #define TOTAL_NUMBER_OF_PIN 19
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

ByteBlock Matrixdigits[30] = {
  { // 0: 0
    B00000000,
    B00011000,
    B00100100,
    B00100100,
    B00100100,
    B00100100,
    B00100100,
    B00011000
  }, { // 1: 1
    B00000000,
    B00001000,
    B00011000,
    B00001000,
    B00001000,
    B00001000,
    B00001000,
    B00011100
  }, { // 2: 2
    B00000000,
    B00011000,
    B00100100,
    B00000100,
    B00001000,
    B00010000,
    B00100000,
    B00111100
  }, { // 3: 3
    B00000000,
    B00011000,
    B00100100,
    B00000100,
    B00001000,
    B00000100,
    B00100100,
    B00011000
  }, { // 4: 4
    B00000000,
    B00001100,
    B00010100,
    B00100100,
    B00111100,
    B00000100,
    B00000100,
    B00000100
  }, { // 5: 5
    B00000000,
    B00111100,
    B00100000,
    B00111000,
    B00000100,
    B00000100,
    B00100100,
    B00011000
  }, { // 6: 6
    B00000000,
    B00011000,
    B00100000,
    B00111000,
    B00100100,
    B00100100,
    B00100100,
    B00011000
  }, { // 7: 7
    B00000000,
    B00111100,
    B00000100,
    B00000100,
    B00001000,
    B00010000,
    B00100000,
    B00100000
  }, { // 8: 8
    B00000000,
    B00011000,
    B00100100,
    B00100100,
    B00011000,
    B00100100,
    B00100100,
    B00011000
  }, { // 9: 9
    B00000000,
    B00011000,
    B00100100,
    B00100100,
    B00100100,
    B00011100,
    B00000100,
    B00011000
  }, { // 10: 10
    B00000000,
    B01000110,
    B11001001,
    B01001001,
    B01001001,
    B01001001,
    B01001001,
    B11100110
  }, { // 11: 11
    B00000000,
    B01000010,
    B11000110,
    B01000010,
    B01000010,
    B01000010,
    B01000010,
    B11100111
  }, { // 12: 12
    B00000000,
    B01000110,
    B11001001,
    B01000001,
    B01000010,
    B01000100,
    B01001000,
    B11101111
  }, { // 13: 13
    B00000000,
    B01000110,
    B11001001,
    B01000001,
    B01000010,
    B01000001,
    B01001001,
    B11100110
  }, { // 14: 14
    B00000000,
    B01000011,
    B11000101,
    B01001001,
    B01001111,
    B01000001,
    B01000001,
    B11100001
  }, { // 15: 15
    B00000000,
    B01001111,
    B11001000,
    B01001110,
    B01000001,
    B01000001,
    B01001001,
    B11100110
  }, { // 16: 16
    B00000000,
    B01000110,
    B11001000,
    B01001110,
    B01001001,
    B01001001,
    B01001001,
    B11100110
  }, { // 17: 17
    B00000000,
    B01001111,
    B11000001,
    B01000001,
    B01000010,
    B01000100,
    B01001000,
    B11101000
  }, { // 18: 18
    B00000000,
    B01000110,
    B11001001,
    B01001001,
    B01000110,
    B01001001,
    B01001001,
    B11100110
  }, { // 19: 19
    B00000000,
    B01000110,
    B11001001,
    B01001001,
    B01001001,
    B01000111,
    B01000001,
    B11100110
  }, { // 20: square
    B00000000,
    B00000000,
    B00111100,
    B00111100,
    B00111100,
    B00111100,
    B00000000,
    B00000000
  }, { // 21: X
    B11000011,
    B11100111,
    B01111110,
    B00111100,
    B00111100,
    B01111110,
    B11100111,
    B11000011
  }, { // 22: !
    B00000000,
    B00011000,
    B00011000,
    B00011000,
    B00000000,
    B00011000,
    B00011000,
    B00000000
  }, { // 23: <
    B00000000,
    B00000000,
    B00001000,
    B00010000,
    B00100000,
    B00010000,
    B00001000,
    B00000000
  }, { // 24: >
    B00000000,
    B00000000,
    B00010000,
    B00001000,
    B00000100,
    B00001000,
    B00010000,
    B00000000
  }, { // 25: N
    B00000000,
    B00100100,
    B00110100,
    B00110100,
    B00101100,
    B00101100,
    B00100100,
    B00000000
  }, { // 26: R1
    B00000000,
    B11000010,
    B10100110,
    B10100010,
    B11000010,
    B10100010,
    B10100010,
    B10100111
  }, { // 27: R2
    B00000000,
    B11000110,
    B10101001,
    B10100001,
    B11000010,
    B10100100,
    B10101000,
    B10101111
  }, { // 28: R3
    B00000000,
    B11000110,
    B10101001,
    B10100001,
    B11000010,
    B10100001,
    B10101001,
    B10100110
  }, { // 29: blank
    B00000000,
    B00000000,
    B00000000,
    B00000000,
    B00000000,
    B00000000,
    B00000000,
    B00000000
  }
};

// Needed for Serial read
bool serialStringAvailable = false;
String serialString = "";

// Maximum of max7219 that can be connected
#define MAX72_SEGMENTS 4

// TODO à optimiser la taille des tableaux à init
boolean arePinsInitialized[TOTAL_NUMBER_OF_PIN] = { false };
Servo availableServos[TOTAL_NUMBER_OF_PIN];

short max72Initialisation[MAX72_SEGMENTS] = {0};
LedController<1,1> available7segMax7219[MAX72_SEGMENTS];
short GetCacheIndexForMax72(short pin) {
  byte i;
  // Reccup l'index de available7segMax7219 correspondant à pin
  for(i = 0; i < MAX72_SEGMENTS; i++) {
    if(max72Initialisation[i] == pin) {
      return i;
    }
  }
  // Si l'index n'a pas été trouvé => nouvelle instance => premier indice libre dans max72Initialisation
  for(i = 0; i < MAX72_SEGMENTS; i++) {
    if(max72Initialisation[i] == 0) {
      max72Initialisation[i] = pin; 
      return i;
    }
  }  
}

#ifdef LCD_ADRESS
// SDA-> A4
// SCL-> A5
// LiquidCrystal_PCF8574 lcd(0x27);
#endif

void setup() {
  Serial.begin(SERIAL_BAUD);
  pinMode(LED_BUILTIN, OUTPUT);
  
  // wait for serial port to connect. Needed for native USB port only (type Leonardo)
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
    digitalWrite(LED_BUILTIN, HIGH);
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
    // :s06134s03160d041a05000m1010111200002 0m111011120200285m141415160010225d070d080:#
    digitalWrite(LED_BUILTIN, HIGH);
    resetSerial();
    return;
  }
  
  // Suppr le delim ':' au début de la chaine
  serialString.remove(0, 1);
  // Test long
  // :s06114s03160d040a05000m1010111200204   0m1701718190010225d070a09000d080:#
  // :s06114s03160d040a05000m1010111200204   0l180060,0874m1717181900224000000000000000000000000d070a09000d080:#

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
      // :m16161718001109876543211#:
      short pinDIN = serialString.substring(3, 5).toInt();
      short pinCS = serialString.substring(5, 7).toInt();
      short pinCLK = serialString.substring(7, 9).toInt();
      short displayOffset = serialString.substring(9, 11).toInt();
      short maxType = serialString.substring(11, 12).toInt();
      short digitLen = serialString.substring(12, 14).toInt();
      short maxIdx = GetCacheIndexForMax72(pinDIN);
      // Use pinDIN as key to allow up to 3 configurations on a single 7seg
      // :m141415160010226:#
      if(arePinsInitialized[pinDIN] == false) {
        available7segMax7219[maxIdx] = LedController<1,1>(pinDIN, pinCLK, pinCS); // DIN,CLK,CS
        available7segMax7219[maxIdx].setIntensity(8);
        available7segMax7219[maxIdx].clearMatrix();
        arePinsInitialized[pinDIN] = true;
      }
      String numberToDisplay = serialString.substring(14, 14 + digitLen);
      if(maxType == 0) {
        // 7 Seg
        // :m101011120000287:#
        // :m101011120000587.12:#
        // :m10101112000043.59:#
        // :m1010111200004   5:#
        byte currDigitPosition = 0;
        available7segMax7219[pinDIN].clearSegment(0);
        for(int i=0; i < digitLen; i++) {
            bool hasDP = i < digitLen - 1 && numberToDisplay[i+ 1] == '.';
            if(numberToDisplay[i] == '.') {
              // Ignore le decimal point car déjà pris en compte par le chiffre précédent
              // d'où l'utilité de currDigitPosition
              continue;
            }
            if(numberToDisplay[i] == ' ') {
              currDigitPosition++;
              continue;
            }
            available7segMax7219[pinDIN].setDigit(
              0, 
              displayOffset + currDigitPosition, 
              numberToDisplay[i], 
              hasDP
            );
            currDigitPosition++;
        }
      } else if(maxType == 1) {
        // Matrix 8x8
        // :m17171819002013:#
        // :m141415160010226:#
        numberToDisplay.trim();
        if(numberToDisplay == "") numberToDisplay = "29";
        available7segMax7219[maxIdx].displayOnSegment(0, 0, Matrixdigits[numberToDisplay.toInt()]);
      } else if(maxType == 2) {
        // Max72xx as digital pin extension
        // exemple start at 143
        // :m1717181900224128000110000072216248255:#
        // :m1717181900224128000128000000000000000:#
        if(digitLen == 24)  {
          available7segMax7219[maxIdx].displayOnSegment(0, 0, {
            (byte)numberToDisplay.substring(0, 3).toInt(),
            (byte)numberToDisplay.substring(3, 6).toInt(),
            (byte)numberToDisplay.substring(6, 9).toInt(),
            (byte)numberToDisplay.substring(9, 12).toInt(),
            (byte)numberToDisplay.substring(12, 15).toInt(),
            (byte)numberToDisplay.substring(15, 18).toInt(),
            (byte)numberToDisplay.substring(18, 21).toInt(),
            (byte)numberToDisplay.substring(21, 24).toInt()
          });
        }        
      }
      serialString.remove(0, 14 + digitLen);
    }
    // lcd
    else if(code == 'l') {
      // :l17010azertyu567ptyuiogdd:#
      // :l17010azertyu567:#
      // :l180200.0330 12.318     98:#
      short stringLen = serialString.substring(3, 6).toInt();
      String stringToDisplay = serialString.substring(6, 6 + stringLen);
      
      // lcd.begin(LCD_COLUMN, LCD_ROW);
      // lcd.home();
      // lcd.clear();
      // lcd.setBacklight(255);
      // lcd.print(stringToDisplay);
      
      serialString.remove(0, 6 + stringLen);
    }
    else {
      // Fallback
      /*Serial.print("Code not supported: ");
      Serial.print(code);
      Serial.println(" - " + serialString);
      */
      digitalWrite(LED_BUILTIN, HIGH);
      resetSerial();
      return;
    }
  }

  // End of Data
  digitalWrite(LED_BUILTIN, LOW);
  resetSerial();
}
