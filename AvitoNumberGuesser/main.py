import socket
import random

HOST = "127.0.0.1"  # Standard loopback interface address (localhost)
PORT = 65432  # Port to listen on (non-privileged ports are > 1023)

MORE_ANSWER = "MORE"
LESS_ANSWER = "LESS"
EQUAL_ANSWER = "EQUAL"

while True:
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.bind((HOST, PORT))
        print(f"Start to listening on port {PORT}")
        s.listen()
        conn, addr = s.accept()
        with conn:
            print(f"Connected by {addr}")

            number = random.randint(0, 10)
            print(f"Number to guess is {number}")

            while True:
                data = conn.recv(1024).decode()

                if (not data.startswith("GUESS")):
                    conn.sendall("WRONG CMD".encode())

                conn_number = int(data.split(' ')[1])

                print(f"Recieved number {conn_number}")

                if (conn_number > number):
                    conn.sendall("LESS".encode())
                elif (conn_number < number):
                    conn.sendall("MORE".encode())
                else:
                    conn.sendall("EQUAL".encode())
                    break

            print(f"Number {number} was guessed by {addr}. Restarting...")
