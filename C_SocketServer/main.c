#include <stdio.h>
#include <windows.h>

#define PORT 45000

int main() {
    SOCKET listener = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);

    struct sockaddr_in addr;
    addr.sin_family = AF_INET;
    addr.sin_port = htons(PORT);

    bind(listener, (struct sockaddr*) & addr, sizeof addr);

    listen(listener, 3);

    while (TRUE) {

    }
    return 0;
}
