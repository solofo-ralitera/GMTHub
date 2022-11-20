//#include <Tone.h>

int pin = 3;
int min = 31;
int max = 210;
void setup() {
  // put your setup code here, to run once:

  // pinMode(pin, OUTPUT);
  // analogWrite(pin, 240);
  tone(pin, 68); //42
}

void loop() {
  // put your main code here, to run repeatedly:
  /*for(int i = min; i < max; i++) {
    tone(pin, i);
    delay(50);
  }*/
}
