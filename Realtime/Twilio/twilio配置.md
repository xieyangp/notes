## 1.配置Twilio的webhook
  

## 2.Twilio的Webhook机制
###  电话进入 -> 访问 URL -> 服务器返回 TwiML -> Twilio 执行指令。

## 3.测试
### 用任意电话拨打 Twilio 号码。查看后端服务日志，确认是否收到 POST 请求。如果通话瞬间挂断，通常是返回的 TwiML 格式错误或服务器地址无法访问。
