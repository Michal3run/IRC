#include <stdio.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <netdb.h>
#include <stdlib.h>
#include <unistd.h>
#include <sys/wait.h>
#include <fcntl.h>
#include <signal.h>
#include <semaphore.h>
#include <dlfcn.h>
#include <errno.h>
#include <string.h>
#include <pthread.h>
#include <time.h>

// gcc -Wall -lpthread

#define MAX_USERS 200
#define MAX_ROOMS 200

struct cln {
	char nick[20];
	int ID; //numer w tablicy, pomaga przy usuwaniu
	int cfd;
	struct sockaddr_in caddr;
};

struct Room{
	char nazwa[16];
	char max_osob[3];
	int ID;
};

char from_user[500]; //wiadomość od klienta

struct cln tabClients[MAX_USERS];
struct Room Rooms[MAX_ROOMS];
int j,roomID=0,userID=0,wyjdz=0;

void nowyKlient(int userID)
{

	char tosend[500]; //wysyłamy listę pokoi przy starcie
	int k;
	
	memset(tosend,0,500);
	strcpy(tosend,"<room_list>");
	
		
	read(tabClients[userID].cfd,from_user, sizeof(from_user));
	int from_user_length=strlen(from_user);	

	//przypisujemy otrzymany nick
	char localnick[20];
	memcpy(localnick, &from_user[10], from_user_length-15 );
	//od 10 znaku (10 zajmuje <new_user>, ciąg o długości: długość-15 (bo 5 od końca zabiera <end>)
	memcpy(tabClients[userID].nick,localnick,from_user_length-15); 
	printf("%s\n",tabClients[userID].nick);

	int tosendlength=16;
	for(k=1;k<MAX_ROOMS;++k)
	{	
		if(Rooms[k].ID!=-1)
		{
			int roomlength=strlen(Rooms[k].nazwa);
			int maxlength=strlen(Rooms[k].max_osob);
			tosendlength=tosendlength+roomlength+maxlength+4;//+4 bo 2x( < i > ) 
			
			strcat(tosend,"<");
			strcat(tosend,Rooms[k].nazwa);
			strcat(tosend,">");
			strcat(tosend,"<");			
			strcat(tosend,Rooms[k].max_osob);
			strcat(tosend,">");

		}

	}
	strcat(tosend,"<end>");	

				
	write(tabClients[userID].cfd,tosend,tosendlength);


//fsync(c.cfd);
//fflush(stdout);
	memset(from_user,0,500);
	
	//printf("%s",from_user);
//fflush(stdout);
while(1){
	memset(from_user,0,500);
	read(tabClients[userID].cfd,from_user, sizeof(from_user));
	//printf("%s\n",from_user);


		
	from_user_length=strlen(from_user);
	printf("%s",from_user);
	fflush(stdout);	
	if(strncmp(from_user,"<new_room>",10)==0)
	{

		if(roomID==MAX_ROOMS)
		{roomID=1;}
		//otrzymany bufor wygląda tak: <new_room><nazwa_pokoju><max_osob><end>
		char tempRoom[21]; //21, bo 16+3+2 znaki w środku
		char tempRoomName[16];
		char tempRoomMax[3];
		memcpy(tempRoom, &from_user[11], from_user_length-17 );//-16 bo 10 znaków jest przed, <end> to 5 po i jeszcze "<" z początku i ">" z końca, które i tak musimy usunąć
//teraz pozostało:  nazwa_pokoju><max_osob 
/*
		char * znak;
		int index;
		znak=strchr(tempRoom,'>');				
		index=znak-tempRoom;

		memcpy(tempRoomName,tempRoom,index);

		memcpy(Rooms[roomID].nazwa,"Main",4);
		memcpy(Rooms[roomID].max_osob,"200",3);
		Rooms[roomID].ID=roomID;
		roomID=roomID+1;
*/
		int i;
		for(i=0;i<=MAX_USERS;++i)
		{
			if(tabClients[i].ID!=-1) //jest podłączony użytkownik			
			{write(tabClients[userID].cfd,from_user,from_user_length);}
		}
	}
	else if(strncmp(from_user,"<delete_user>",13)==0)
	{
		printf("%s","Odlaczony od serwera: ");
		printf("%s",tabClients[userID].nick);	
		memcpy(tabClients[userID].nick,"",0);
		tabClients[userID].ID=-1;
			
		
		wyjdz=1;
		
	}
		
	else if(strncmp(from_user,"<new_message>",13)==0)
	{	
		for(k=0;k<MAX_USERS;++k)
		{
			if(tabClients[k].ID!=-1)		
			{write(tabClients[k].cfd,from_user,from_user_length);
			printf("%s %d\n",from_user,from_user_length);
fflush(stdout);
			}
		}
	}
	
	if(wyjdz==1)
	{		
		//break;
	}

	}

//close(tabClients[userID].cfd);
return;
}

int main(int argc, char* argv[]){ //uruchamiamy: ./serwer numer_portu
	
	if(argc!=2)
	{
		printf("Zła liczba argumentów (podaj: numer_portu)");
		exit(1);
	}else{

	for(j=0;j<MAX_USERS;++j)
	{		
		tabClients[j].ID=-1;
	}
		
	for(j=0;j<MAX_ROOMS;++j)
	{
		Rooms[j].ID=-1;
	}
	//tworzymy pokój główny
	memcpy(Rooms[roomID].nazwa,"Main",4);
	memcpy(Rooms[roomID].max_osob,"200",3);
	Rooms[roomID].ID=roomID;
	roomID=roomID+1;

	memcpy(Rooms[roomID].nazwa,"Football",8);
	memcpy(Rooms[roomID].max_osob,"100",3);
	Rooms[roomID].ID=roomID;
	roomID=roomID+1;

	memcpy(Rooms[roomID].nazwa,"Computer games",14);
	memcpy(Rooms[roomID].max_osob,"80",2);
	Rooms[roomID].ID=roomID;
	roomID=roomID+1;
/*
char * znak;
		int index;
char str[]="Main><200";
		znak=strchr(str,'>');				
		index=znak-str;	*/	
	int fd = socket(PF_INET,SOCK_STREAM,0);
	int on=1;
/*char a[10];
char b[10];
memcpy(b,&str[index+2],1);
//printf("%s",b);
fflush(stdout);*/
	socklen_t slt;

	struct sockaddr_in sa;
	sa.sin_family=PF_INET;
	sa.sin_port=htons(atoi(argv[1]));
	sa.sin_addr.s_addr=INADDR_ANY;
	
	bind(fd,(struct sockaddr*)&sa,sizeof(sa));
	listen(fd,MAX_USERS);	
	
	
	while(1)
	{
		
		slt=sizeof(struct sockaddr_in); //rozmiar struktury adresowej
		while(tabClients[userID].ID!=-1)//sprawdzamy czy klient w tablicy o tym indeksie nadal jest podłączony, czy nie
		{
			userID=userID+1;
			if(userID==MAX_USERS)
			 {userID=0;}
		}			
	
		tabClients[userID].cfd = accept(fd,(struct sockaddr*)&tabClients[userID].caddr,&slt);
		tabClients[userID].ID=userID;
			
		//tworzymy proces potomny
		if(fork()==0)
		{	printf("%s","Proces potomny, użytkownik : ");
			nowyKlient(userID);					
		
		}
		
		userID=userID+1;		
		if(userID==MAX_USERS)
		{userID=0;}	

		


		//po dodaniu pokoju
//		if(roomID==MAX_ROOMS)
//		{roomID=1;}//główny zawsze zostawiamy

	}

	close(fd);
	return 0;
	}

}
