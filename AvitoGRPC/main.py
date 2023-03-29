from concurrent import futures
import logging

import grpc
import auth_pb2_grpc
import auth_pb2


class AuthService(auth_pb2_grpc.AuthServiceServicer):
    def __init__(self):
        self.__users = ["vladafon", "admin"]

    def IsAuth(self, request, context):
        print(f"Recieved Auth request for {request.user_name}")

        result = request.user_name in self.__users

        return auth_pb2.Response(is_auth_result=result)

def serve():
    port = '50051'
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    auth_pb2_grpc.add_AuthServiceServicer_to_server(AuthService(), server)
    server.add_insecure_port('[::]:' + port)
    server.start()
    print("Server started, listening on " + port)
    server.wait_for_termination()


if __name__ == '__main__':
    logging.basicConfig()
    serve()