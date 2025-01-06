import requests
import time

class SignalTester:
    def __init__(self):
        self.base_url = "https://ylf2025.iottalk.tw/signal"
        self.headers = {
            "Content-Type": "text/plain",
            "Authorization": "Bearer fa1a4509915146fdb07ae71645b561cf"
        }

    def send_signal(self, signal: int) -> None:
        """發送信號到IOTTalk服務器"""
        try:
            response = requests.post(
                self.base_url,
                data=str(signal),
                headers=self.headers
            )
            if response.status_code == 200:
                print(f"信號發送成功: {signal}")
                print(f"服務器回應: {response.text}")
            else:
                print(f"發送失敗，狀態碼: {response.status_code}")
                print(f"錯誤信息: {response.text}")
        except Exception as e:
            print(f"發送過程出錯: {str(e)}")

    def get_signal(self) -> None:
        """獲取當前信號值"""
        try:
            response = requests.get(
                self.base_url,
                headers=self.headers
            )
            if response.status_code == 200:
                print(f"當前信號值: {response.text}")
            else:
                print(f"獲取失敗，狀態碼: {response.status_code}")
        except Exception as e:
            print(f"獲取過程出錯: {str(e)}")

def main():
    tester = SignalTester()
    
    while True:
        print("\n請選擇操作:")
        print("1. 發送信號 1")
        print("2. 發送信號 0")
        print("3. 獲取當前信號")
        print("4. 退出")
        
        choice = input("請輸入選項 (1-4): ")
        
        if choice == "1":
            tester.send_signal(1)
        elif choice == "2":
            tester.send_signal(0)
        elif choice == "3":
            tester.get_signal()
        elif choice == "4":
            print("程序結束")
            break
        else:
            print("無效的選擇，請重試")
        
        time.sleep(0.5)  # 短暫延遲以避免請求過於頻繁

if __name__ == "__main__":
    main() 