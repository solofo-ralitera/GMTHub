#include <Servo.h>
Servo servo1;
void setup() 
{
  Serial.begin(9600);
  servo1.attach(10);   
}
int i = 0;
void loop() 
{ 
  if(i > 90) i = 0;
  servo1.write(i);
  i++;
  delay(100);
}