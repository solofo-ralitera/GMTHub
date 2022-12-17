// Id for this board, increment this value for each board used by GMTHub
#define BOARD_NUMBER "1"

// #define ANALOG_OUTPUT    // Use analog output?, comment if none

// #define DIGITAL_OUTPUT   // Use digital output?, comment if none

// Number of device using tone, comment if none (maximum = number of timer interupt - 1, because timer1 is used by servo)
#define USE_TONE 2 

// Number of device using servo, comment if none
#define USE_SERVO 4

// Number of connected max7219 (including: 7seg, dot matrix and led extension), comment if none
#define MAX72_SEGMENTS 2
#define MAX72_7SEG     // Use 7seg?
// #define MAX72_MATRIX   // Use dot matrix?
#define MAX72_LEDEXT      // Use led extension?
#define MAX72_7SEG_INTENSITY 3

// Lcd display configuration
#define LCD_ADRESS 0x27 // Use LCD?, comment if none
#define LCD_COLUMN 20
#define LCD_ROW 4

// Serial port configuration
#define SERIAL_BAUD 9600
#define SERIAL_EOL '#'

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

// Scaning acknoledgement message
#define ACK_SCAN "ack_gmtscan_" BOARD "-" BOARD_NUMBER
#define ACK_READY "ready_" BOARD "-" BOARD_NUMBER

// Needed for Serial read
bool serialStringAvailable = false;
String serialString = "";

boolean arePinsInitialized[TOTAL_NUMBER_OF_PIN] = { false };

#ifdef USE_SERVO
#include <Servo.h>
short servoInitialisation[USE_SERVO] = {0};
Servo availableServos[USE_SERVO];
#endif

#ifdef USE_TONE
#include <Tone.h>
short toneInitialisation[USE_TONE] = {0};
Tone availabileTones[USE_TONE];
#endif

#ifdef MAX72_SEGMENTS
#include "LedController.hpp" // https://github.com/noah1510/LedController
#ifdef MAX72_MATRIX
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
#endif
short max72Initialisation[MAX72_SEGMENTS] = {0};
LedController<1,1> available7segMax7219[MAX72_SEGMENTS];
#endif

#ifdef LCD_ADRESS
// SDA-> A4
// SCL-> A5
#include "LiquidCrystal_I2C_DFRobot.h"
LiquidCrystal_I2C I2CLCD(LCD_ADRESS, LCD_COLUMN, LCD_ROW);
#endif

short GetCacheIndexForDevice(short pin, short pinInitialisation[], short deviceCount) {
  byte i;
  // Reccup l'index de available7segMax7219 correspondant à pin
  for(i = 0; i < deviceCount; i++) {
    if(pinInitialisation[i] == pin) {
      return i;
    }
  }
  // Si l'index n'a pas été trouvé => nouvelle instance => premier indice libre dans pinInitialisation
  for(i = 0; i < deviceCount; i++) {
    if(pinInitialisation[i] == 0) {
      pinInitialisation[i] = pin; 
      return i;
    }
  }  
}

void setup() {
  Serial.begin(SERIAL_BAUD);
  pinMode(LED_BUILTIN, OUTPUT);
  
  #ifdef LCD_ADRESS
  I2CLCD.init();
	I2CLCD.backlight();
  #endif

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
  // serialString.trim();
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
    digitalWrite(LED_BUILTIN, HIGH);
    resetSerial();
    return;
  }
  
  // Suppr le delim ':' au début de la chaine
  serialString.remove(0, 1);
  
  // 4 :length min des commandes
  while(serialString.length() > 4) {
    byte pin = serialString.substring(1, 3).toInt();
    char code = serialString.charAt(0);

    // Digital output
    if(code == 'd') {
      #ifdef DIGITAL_OUTPUT
      if(arePinsInitialized[pin] == false) {
        pinMode(pin, OUTPUT);
        arePinsInitialized[pin] = true;
      }
      byte state = serialString.substring(3, 4).toInt();
      digitalWrite(pin, state ? HIGH : LOW);
      #endif
      serialString.remove(0, 4);
    }
    // Analog output
    else if(code == 'a') {
      #ifdef ANALOG_OUTPUT
      if(arePinsInitialized[pin] == false) {
        pinMode(pin, OUTPUT);
        arePinsInitialized[pin] = true;
      }      
      short value = serialString.substring(3, 6).toInt();
      analogWrite(pin, value); 
      #endif
      serialString.remove(0, 6);
    }
    // Servo
    else if(code == 's') {
      // :s10070:#
      #ifdef USE_SERVO
      short servoIdx = GetCacheIndexForDevice(pin, servoInitialisation, USE_SERVO);
      if(arePinsInitialized[pin] == false) {
        availableServos[servoIdx].attach(pin);
        arePinsInitialized[pin] = true;
      }
      short pulseWidth = serialString.substring(3, 7).toInt();
      availableServos[servoIdx].write(pulseWidth); 
      #endif
      serialString.remove(0, 7);
    }
    // Tone (device driven by frequency)
    else if(code == 't') {
      #ifdef USE_TONE
      // :t030055:#
      short toneIdx = GetCacheIndexForDevice(pin, toneInitialisation, USE_TONE);
      if(arePinsInitialized[pin] == false) {
        availabileTones[toneIdx].begin(pin);
        arePinsInitialized[pin] = true;
      }
      short frequency = serialString.substring(3, 7).toInt();
      if(frequency == 0) {
        availabileTones[toneIdx].stop(); 
      } else {
        availabileTones[toneIdx].play(frequency); 
      }
      #endif
      serialString.remove(0, 7);
    }
    // Max 7seg Display mode SPI
    else if(code == 'm') {
      byte csPin = serialString.substring(3, 5).toInt();
      byte displayOffset = serialString.substring(5, 7).toInt();
      byte maxType = serialString.substring(7, 8).toInt();
      byte digitLen = serialString.substring(8, 10).toInt();
      #ifdef MAX72_SEGMENTS
      // Use csPin as key to allow up to 3 value on a single max7219
      short maxIdx = GetCacheIndexForDevice(csPin, max72Initialisation, MAX72_SEGMENTS);
      if(arePinsInitialized[csPin] == false) {
        available7segMax7219[maxIdx] = LedController<1,1>(csPin); // Use SPI hardware instead of DIN,CLK,CS
        available7segMax7219[maxIdx].setIntensity(MAX72_7SEG_INTENSITY);
        available7segMax7219[maxIdx].clearMatrix();
        arePinsInitialized[csPin] = true;
      }
      String numberToDisplay = serialString.substring(10, 10 + digitLen);
      if(maxType == 0) {
        #ifdef MAX72_7SEG
        // 7 Seg
        byte currDigitPosition = 0;
        for(int i=0; i < digitLen; i++) {
            bool hasDP = i < digitLen - 1 && numberToDisplay[i+ 1] == '.';
            if(numberToDisplay[i] == '.') {
              // Ignore le decimal point car déjà pris en compte par le chiffre précédent
              // d'où l'utilité de currDigitPosition
              continue;
            }
            if(numberToDisplay[i] == ' ') {
              available7segMax7219[maxIdx].setChar(
                0, 
                displayOffset + currDigitPosition, 
                numberToDisplay[i], 
                hasDP
              );
            } else {
              available7segMax7219[maxIdx].setDigit(
                0, 
                displayOffset + currDigitPosition, 
                numberToDisplay[i], 
                hasDP
              );
            }
            currDigitPosition++;
        }
        #endif
      } else if(maxType == 1) {
        // Dot Matrix 8x8
        #ifdef MAX72_MATRIX
        numberToDisplay.trim();
        if(numberToDisplay == "") numberToDisplay = "29"; // 29 correspond à vide dans le tableau des caractères du matrix (Matrixdigits)
        available7segMax7219[maxIdx].displayOnSegment(0, 0, Matrixdigits[numberToDisplay.toInt()]);
        #endif
      } else if(maxType == 2) {
        // Max72xx as led extension
        #ifdef MAX72_LEDEXT
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
        #endif
      }
      #endif
      serialString.remove(0, 10 + digitLen);
    }
    // lcd
    else if(code == 'l') {
      short stringLen = serialString.substring(3, 6).toInt();
      #ifdef LCD_ADRESS
      String stringToDisplay = serialString.substring(6, 6 + stringLen);
      byte currColumn = 0;
      byte currentLine = 0;
      byte charPrinted = 0;
      I2CLCD.setCursor(0, 0); 
      for (byte s = 0; s < stringToDisplay.length(); s++)
      { 
        if(stringToDisplay[s] == '|') 
        { 
          // Efface les colonnes restants de la ligne
          for(byte c = currColumn; c < LCD_COLUMN; c++) {
            I2CLCD.print(' ');  
            charPrinted++;          
          }
          currColumn = 0;
          currentLine++; 
          if(currentLine >= LCD_ROW) {
            // Dépasse l'affichage du lcd            
            break;
          }
          I2CLCD.setCursor(0, currentLine); 
          continue;
        }
        currColumn++;
        // Cas ou la ligne dépasse l'affichage du lcd: ignore
        if(currColumn <= LCD_COLUMN) {
          I2CLCD.print(stringToDisplay[s]);
        }
        charPrinted++;
      }
      // Efface les vides restants
      byte remaining = (LCD_COLUMN * LCD_ROW) - charPrinted;
      for(byte r = 0; r < remaining; r++) {
        I2CLCD.print(' ');            
        currColumn++;
        if(currColumn >= LCD_COLUMN) {
          currColumn = 0;
          currentLine++; 
          I2CLCD.setCursor(0, currentLine); 
        }
      }
      #endif
      serialString.remove(0, 6 + stringLen);
    }
    else {
      // Fallback
      digitalWrite(LED_BUILTIN, HIGH);
      resetSerial();
      return;
    }
  }

  // End of Data
  digitalWrite(LED_BUILTIN, LOW);
  resetSerial();
}
