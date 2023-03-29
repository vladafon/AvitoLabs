import socket

HOST = "127.0.0.1"  # The server's hostname or IP address
PORT = 65432  # The port used by the server

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    s.connect((HOST, PORT))

    is_guessed = False

    while not is_guessed:
        print("Enter a number")
        input_number = input()

        input_number_int = -1;

        try:
            input_number_int = int(input_number)
        except ValueError:
            print("Wrong input")
            continue

        s.sendall(f"GUESS {input_number_int}".encode())
        data = s.recv(1024)
        print(f"Received {data!r}")

        if (data.decode() == "EQUAL"):
            print(f"You won!")
            is_guessed = True
