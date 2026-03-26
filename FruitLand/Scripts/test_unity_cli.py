import socket
import json
import struct
import sys

def send_command(host, port, command):
    try:
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        sock.settimeout(5)
        sock.connect((host, port))
        
        data = json.dumps(command).encode('utf-8')
        length = struct.pack('<I', len(data))
        sock.sendall(length + data)
        
        resp_length_data = sock.recv(4)
        if len(resp_length_data) < 4:
            return {"error": "Failed to read response length"}
        
        resp_length = struct.unpack('<I', resp_length_data)[0]
        response_data = b''
        while len(response_data) < resp_length:
            chunk = sock.recv(min(4096, resp_length - len(response_data)))
            if not chunk:
                break
            response_data += chunk
        
        sock.close()
        return json.loads(response_data.decode('utf-8'))
    except Exception as e:
        return {"error": str(e)}

if __name__ == "__main__":
    host = "localhost"
    port = 6400
    
    command = {
        "id": "test-1",
        "type": "get_editor_state"
    }
    
    if len(sys.argv) > 1:
        command["type"] = sys.argv[1]
    if len(sys.argv) > 2:
        command["params"] = json.loads(sys.argv[2])
    
    result = send_command(host, port, command)
    print(json.dumps(result, indent=2))
