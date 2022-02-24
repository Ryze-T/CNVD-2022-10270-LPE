# CNVD-2022-10270-LPE

## 用法

基于向日葵RCE的本地权限提升，无需指定端口

Usage：sunloginLPE.exe Cmd [sunloginClientPath]

sunloginClientPath 选填，若不是默认安装路径则需要指定，默认安装路径：C:\Program Files\Oray\SunLogin\SunloginClient

如 sunloginLPE.exe "whoami"

![image-20220224102724040](https://gitee.com/tboom_is_here/pic/raw/master/2021-10-21/20220224102724.png)

如 sunloginLPE.exe "net user"

![image-20220224102826549](https://gitee.com/tboom_is_here/pic/raw/master/2021-10-21/20220224102826.png)

若指定路径如 sunloginLPE.exe "whoami" "C:\Program Files\Oray\SunLogin\SunloginClient"

![image-20220224102912848](https://gitee.com/tboom_is_here/pic/raw/master/2021-10-21/20220224102912.png)

## 无需指定端口的原因

现在流传的方法是扫描40000-60000的端口，但实际上跟着IDA分析过程会看到生成端口并绑定服务时有写入的动作，最终写入的文件是

sunlogin_service.xxx.log，指定端口如下：

![image-20220224103301658](https://gitee.com/tboom_is_here/pic/raw/master/2021-10-21/20220224103301.png)

因此只需要在去读最新的日志文件并进行正则匹配，就不需要进行端口扫描。
