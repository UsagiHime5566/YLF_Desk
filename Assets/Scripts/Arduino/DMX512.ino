#include <ArduinoDMX.h>

/*
  DMX Blink

  This sketch toggles the value of DMX channel 1 between 255 and 0.

  Circuit:
   - DMX light
   - MKR board
   - MKR 485 shield
     - ISO GND connected to DMX light GND (pin 1)
     - Y connected to DMX light Data + (pin 2)
     - Z connected to DMX light Data - (pin 3)
     - Jumper positions
       - Z \/\/ Y set to ON

  created 5 July 2018
  by Sandeep Mistry
*/

#include <ArduinoRS485.h> // the ArduinoDMX library depends on ArduinoRS485
#include <ArduinoDMX.h>

const int universeSize = 16;
int ledPin = 13;  // LED connected to digital pin 13
int inPin = 7;    // pushbutton connected to digital pin 7
int val = 0;      // variable to store the read value
int incomingByte = -1; // for incoming serial data


void setup() {
  pinMode(ledPin, OUTPUT);  // sets the digital pin 13 as output
  pinMode(inPin, INPUT);    // sets the digital pin 7 as input
  Serial.begin(9600);
  while (!Serial);

  // initialize the DMX library with the universe size
  if (!DMX.begin(universeSize)) {
    Serial.println("Failed to initialize DMX!");
    while (1); // wait for ever
  }
}

void loop() {
  
  
  val = digitalRead(inPin);   // read the input pin


    if(val==1) //// 按鈕on發給unity y
    {
      Serial.println("y"); 
    }

    if(val==0) //// 按鈕off發給unity n
    {
      Serial.println("n"); 
    }


  if (Serial.available() > 0) {
  // read the incoming byte:
  incomingByte = Serial.read();
  }

  
  if(incomingByte==49) //// unity傳來1
  {
  // set channel 1 value to 255
  DMX.beginTransmission();
  DMX.write(1, 255);
  DMX.endTransmission();
  digitalWrite(ledPin, HIGH);  // sets the LED to the button's value
  delay(8000); //亮燈8s
  }


  if(incomingByte==48) //// unity傳來0
  {
  // set channel 1 value to 0
  DMX.beginTransmission();
  DMX.write(1, 0);
  DMX.endTransmission();
  digitalWrite(ledPin, LOW);  // sets the LED to the button's value
  delay(20);
  }

delay(20);
}
