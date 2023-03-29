from google.protobuf import wrappers_pb2 as _wrappers_pb2
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from typing import ClassVar as _ClassVar, Optional as _Optional

DESCRIPTOR: _descriptor.FileDescriptor

class Request(_message.Message):
    __slots__ = ["user_name"]
    USER_NAME_FIELD_NUMBER: _ClassVar[int]
    user_name: str
    def __init__(self, user_name: _Optional[str] = ...) -> None: ...

class Response(_message.Message):
    __slots__ = ["is_auth_result"]
    IS_AUTH_RESULT_FIELD_NUMBER: _ClassVar[int]
    is_auth_result: bool
    def __init__(self, is_auth_result: bool = ...) -> None: ...
