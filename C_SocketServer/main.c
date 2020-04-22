#include <winsock2.h>
#include <stdio.h>
#pragma comment(lib, "Ws2_32.lib")
#define PORT 45678
#define BUF_SIZE 1024

int main() {
    WSADATA wsaData;
    WSAStartup(MAKEWORD(2, 2), &wsaData);

    SOCKET sock = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);

    struct sockaddr_in recv_addr;
    recv_addr.sin_family = AF_INET;
    recv_addr.sin_port = htons(PORT);
    recv_addr.sin_addr.s_addr = htonl(INADDR_ANY);

    bind(sock, (SOCKADDR*)&recv_addr, sizeof recv_addr);

    char buf[BUF_SIZE];

    struct sockaddr_in sender_addr;
    int sender_addr_size = sizeof(sender_addr);
    listen(sock, 10);
    SOCKET clientSock = accept(sock, (SOCKADDR*)&sender_addr, &sender_addr_size);
    // connect(sock, (SOCKADDR*)&sender_addr, &sender_addr_size);
    // send() // tcp
    // sendto() // udp
    recv(clientSock, buf, BUF_SIZE, 0);

    while (TRUE) {
        int len = recvfrom(
            sock, // socket
            buf, BUF_SIZE, // buffer info
            0, // nothing
            (SOCKADDR*)&sender_addr, &sender_addr_size // client's EndPoint
        );
        buf[len] = '\0';
        if (strcmp(buf, "die") == 0) {
            printf("Goodbye\n");
            break;
        }
        printf("%s\n", buf);
    }
    
    closesocket(sock);

    WSACleanup();

    return 0;
}