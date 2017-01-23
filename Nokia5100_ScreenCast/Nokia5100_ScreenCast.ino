
/* 84x48 LCD Defines: */
#define LCD_WIDTH   84 // Note: x-coordinates go wide
#define LCD_HEIGHT  48 // Note: y-coordinates go high
 
int x = 0;
int y = 0;


void setup()
{
  Serial.begin(250000);
  lcdBegin(); // This will setup our pins, and initialize the LCD
  updateDisplay(); // with displayMap untouched, SFE logo
  setContrast(40); // Good values range from 40-60
  delay(1000);
  clearDisplay(0);
  updateDisplay();
  delay(500);
  setStr("Hello!", 25, 17, 1);
  updateDisplay();
}


void loop()
{
  
  if (Serial.available())
  {
    char c = Serial.read();
    int digit = (int)c;
    //clearDisplay(0);

    bool b1 = (1&digit) >= 1;
    bool b2 = (2&digit) >= 1;
    bool b3 = (4&digit) >= 1;
    bool b4 = (8&digit) >= 1;
    bool b5 = (16&digit) >= 1;
    bool b6 = (32&digit) >= 1;
    bool b7 = (64&digit) >= 1;
    bool b8 = (128&digit) >= 1;

    if(!b8){
      //Serial.println("WRITE");
      writePixel(b7);
      writePixel(b6);
      writePixel(b5);
      writePixel(b4);
      writePixel(b3);
      writePixel(b2);
      writePixel(b1);
    }
    else{
     if(b1){
        updateDisplay(); 
        //Serial.println("UPDATE");
     }
     if(b2){
        clearDisplay(0);
        x=0;
        y=0;
        //Serial.println("CLEAR");
     }
    }

//    Serial.print(c);
//    Serial.print('|');
//    Serial.print(b8?'1':'0');
//    Serial.print(b7?'1':'0');
//    Serial.print(b6?'1':'0');
//    Serial.print(b5?'1':'0');
//    Serial.print(b4?'1':'0');
//    Serial.print(b3?'1':'0');
//    Serial.print(b2?'1':'0');
//    Serial.println(b1?'1':'0');
  }
}

void writePixel(bool value){
  setPixel(x, y, value);
  x++;
  
  if(x >= LCD_WIDTH){
    x = 0; 
    y++;
  }
  if(y >= LCD_HEIGHT){
    y = 0; 
  }
}




