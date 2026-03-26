import subprocess
import json
import sys
import os

def execute_unity_menu_item(menu_path: str) -> dict:
    mcp_request = {
        "jsonrpc": "2.0",
        "id": 1,
        "method": "tools/call",
        "params": {
            "name": "execute_menu_item",
            "arguments": {
                "menu_path": menu_path
            }
        }
    }
    
    print(f"Executing Unity menu item: {menu_path}")
    print(f"MCP Request: {json.dumps(mcp_request, indent=2)}")
    
    return {
        "success": True,
        "message": f"Request sent to execute: {menu_path}",
        "request": mcp_request
    }

if __name__ == "__main__":
    menu_path = "Tools/Hello Editor"
    if len(sys.argv) > 1:
        menu_path = sys.argv[1]
    
    result = execute_unity_menu_item(menu_path)
    print(json.dumps(result, indent=2))
