#include <MFRC522.h>
#include <SPI.h>

#define GREEN_LED 7
#define RED_LED 6

#define RST_PIN 9
#define SS_PIN 10 

MFRC522 rfid(SS_PIN, RST_PIN); // Instance of the class

byte nuidPICC[4];

String deviceId = "-1";
// - If classroom not assigned. Thrown exception in ARDIS.
String classroom = "-";

void setup() {
  pinMode(GREEN_LED, OUTPUT);
  pinMode(RED_LED, OUTPUT);
  Serial.begin(9600);
  SPI.begin(); // Init SPI bus
  rfid.PCD_Init(); // Init MFRC522 
  GetDeviceId();
}

void GetDeviceId()
{
  if(Serial.available())
  {
    if(Serial.readString() == "?")
    {
      while(true)
      {
        delay(10);
        if(Serial.available())
        {
          if(Serial.readString() != "?")
          {
            return;
          }
        }
      }
    }
  }
  Serial.println("?");
  while(true)
  {
    delay(10);
    if(Serial.available())
    {
      String s = Serial.readString();
      deviceId = "";   
      for(int i = 0; i < s.length() - 1; i++)
      {
        deviceId += s[i];
      }
      return;
    }
  }  
}

void loop() {
  if(Serial.available())
  {
    String response = Serial.readString();
    ParseResponse(response);
  }
  
  // Look for new cards
  if ( ! rfid.PICC_IsNewCardPresent())
  {
    digitalWrite(GREEN_LED, LOW);
    digitalWrite(RED_LED, LOW);
    return;
  }

  // Verify if the NUID has been readed
  if ( ! rfid.PICC_ReadCardSerial())
  {
    digitalWrite(GREEN_LED, LOW);
    digitalWrite(RED_LED, HIGH);
    delay(1000);
    return;
  }
  
  if(classroom == "-")
  {
    Serial.print("!Device ");
    Serial.print(deviceId);
    Serial.println(": Please, assign the classroom!");
    delay(1000);
    return;
  }
  
  if (rfid.uid.uidByte[0] != nuidPICC[0] || 
    rfid.uid.uidByte[1] != nuidPICC[1] || 
    rfid.uid.uidByte[2] != nuidPICC[2] || 
    rfid.uid.uidByte[3] != nuidPICC[3] ) {

    // Store NUID into nuidPICC array
    for (byte i = 0; i < 4; i++) {
      nuidPICC[i] = rfid.uid.uidByte[i];
    }
    Serial.print(classroom + "|");
    printHex(rfid.uid.uidByte, rfid.uid.size);
    Serial.println();
  }

  // Halt PICC
  rfid.PICC_HaltA();

  // Stop encryption on PCD
  rfid.PCD_StopCrypto1();

  digitalWrite(GREEN_LED, HIGH);
  delay(250);
}

void ParseResponse(String response)
{
  String s;
  for(int i = 1; i < deviceId.length()+1; i++)
  {
    s += response[i];
  }
  if(s == deviceId)
  {
     char commandNum = response[deviceId.length()+2];
     s = "";
     for(int i = deviceId.length()+4; i < response.length()-2; i++)
     {
        s += response[i];
     }
     switch(commandNum)
     {
      case '0':
        deviceId = s;
        return;
      case '1':
        classroom = s;
        return;
      default:
        Serial.println("Device " + deviceId + ": Command not recognized!");
        return;
     }
  } else
  {
    s = "";
    for(int i = 1; i < 3; i++)
    {
      s+=response[i];
    }
    if(s == "-1")
    {
      Serial.println("|"+ deviceId + "|" + classroom + "|");
    }
  }
}

/**
 * Helper routine to dump a byte array as hex values to Serial. 
 */
void printHex(byte *buffer, byte bufferSize) {  
  for (byte i = 0; i < bufferSize; i++) {
    Serial.print(buffer[i] < 0x10 ? "0" : "");
    Serial.print(buffer[i], HEX);
  }
}

/**
 * Helper routine to dump a byte array as dec values to Serial.
 */
void printDec(byte *buffer, byte bufferSize) {
  for (byte i = 0; i < bufferSize; i++) {
    Serial.print(buffer[i] < 0x10 ? " 0" : " ");
    Serial.print(buffer[i], DEC);
  }
}

