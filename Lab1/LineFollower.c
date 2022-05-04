/*
 * SerialCharRead.c
 *
 * Created: 4/10/2022 3:23:46 PM
 * Author : Kurt Baehr
 Collaborated with Lauren Crist
*/

#include "F_cpu_lib.h" //Set clock frequency
#include <avr/io.h>
#include <util/delay.h>
#include "LCD_lib.h" //LCD library
#include "Serial_lib2.h" //Serial Communication library
#include "Motor_PWM_lib.h" //Motor Control library
#include "A2D_lib.h" //Analog to Digital Conversion library
#include "tic_toc_lib.h" //Timer library

// initializing motor references and rotation direction
uint8_t leftMotor = 1;
uint8_t rightMotor = 0;
uint8_t forwardDir = 0;
uint8_t reverseDir = 1;

int main(void)
{
	DDRC |= 1 << PORTC2; //Set PortC2 to output
	char mode; //Character sent by VS using Serial
		
	LCD_init();
	USART_vInit();
	HBridgeInit();
	
	sei(); //Enable interrupts
	
	// Print a welcome message on LCD 
	LCDGoToPosition(1,1);
	LCDSendString("Good morning");
	
	while(1)
	{
		
		mode = USART_vReceiveByte(); //set mode to received byte
		
		LCDGoToPosition(1,3);
		LCDSendString(mode);
		
		//Data structure to react to characters sent through serial, commands motors accordingly
		switch(mode)
		{
			case 'S':
			{
				LCDClearScreen();
				HBridgeCommand(leftMotor, 100, forwardDir);
				HBridgeCommand(rightMotor, 100, forwardDir);
				LCDGoToPosition(1,1);
				LCDSendString("Going straight");
				break;
				
			}
			case 'L':
			{	
				LCDClearScreen();
				HBridgeCommand(leftMotor, 20, forwardDir);
				HBridgeCommand(rightMotor, 80, forwardDir);
				LCDGoToPosition(1,1);
				LCDSendString("Turning left NOW"); 
				break;
				
			}
			case 'K':
			{
				LCDClearScreen();
				HBridgeCommand(leftMotor, 30, forwardDir);
				HBridgeCommand(rightMotor, 85, forwardDir);
				LCDGoToPosition(1,1);
				LCDSendString("Turning left SLOW");
				break;
				
			}
			case 'R':
			{
				LCDClearScreen();
				HBridgeCommand(leftMotor, 80, forwardDir);
				HBridgeCommand(rightMotor, 20, forwardDir);
				LCDGoToPosition(1,1);
				LCDSendString("Turning right NOW");
				break;
				
			}
			case 'T':
			{
				LCDClearScreen();
				HBridgeCommand(leftMotor, 85, forwardDir);
				HBridgeCommand(rightMotor, 30, forwardDir);
				LCDGoToPosition(1,1);
				LCDSendString("Turning right SLOW");
				break;
			}
			case 'P':
			{
				LCDClearScreen();
				HBridgeCommand(leftMotor, 0, forwardDir);
				HBridgeCommand(rightMotor, 0, forwardDir);
				LCDGoToPosition(1,1);
				LCDSendString("Stopping...");
				break;
			}
		}
	}
}

