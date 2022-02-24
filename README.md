# CNVD-2022-10270-LPE

## 用法

基于向日葵RCE的本地权限提升，无需指定端口

Usage：sunloginLPE.exe Cmd [sunloginClientPath]

sunloginClientPath 选填，若不是默认安装路径则需要指定，默认安装路径：C:\Program Files\Oray\SunLogin\SunloginClient

如 sunloginLPE.exe "whoami"
![20220224102724](https://user-images.githubusercontent.com/76553352/155446699-49771a7b-2411-46b2-822b-ef353e21ebbe.png)

如 sunloginLPE.exe "net user"
![image](https://user-images.githubusercontent.com/76553352/155446749-31153e71-5eed-4e15-8bd6-aaa7ef727bde.png)

若指定路径如 sunloginLPE.exe "whoami" "C:\Program Files\Oray\SunLogin\SunloginClient"

![image](https://user-images.githubusercontent.com/76553352/155446775-7e87cc1f-43ad-4f25-a559-391b6fb3afaf.png)


## 无需指定端口的原因

现在流传的方法是扫描40000-60000的端口，但实际上跟着IDA分析过程会看到生成端口并绑定服务时有写入的动作，最终写入的文件是

sunlogin_service.xxx.log，指定端口如下：

![image](https://user-images.githubusercontent.com/76553352/155446806-8e6b1547-c46b-4b81-a753-1591901030fe.png)


因此只需要在去读最新的日志文件并进行正则匹配，就不需要进行端口扫描。
