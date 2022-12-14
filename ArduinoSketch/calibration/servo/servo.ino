#include <Servo.h>
Servo servo1;
void setup() 
{
  Serial.begin(9600);
  servo1.attach(6);   
  servo1.write(90);
}
int i = 0;
void loop() 
{ 
  /*servo1.write(0);
  delay(500);
  servo1.write(45);
  delay(500);
  servo1.write(90);
  delay(500);*/

}