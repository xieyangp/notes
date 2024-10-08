## 安装Freepbx 16.0
### 1.先安装教程进行安装: 
[freepbx安装教程](https://cloudinfrastructureservices.co.uk/how-to-install-freepbx-on-ubuntu-22-04-23-04)
### 2.配置asterisk时只需要配置完前三步即可：
![image/Freepbx/1721376495665.jpg](https://github.com/xieyangp/notes/blob/main/image/Freepbx/1721376495665.jpg)
### 3.安装LAMP服务器，再安装完后可以执行php -v查看php的版本是否为 7.4，如果不是7.4在之后安装FreePBX会出现版本不适配的情况
![4](https://github.com/xieyangp/notes/blob/main/image/Freepbx/1721376727455.jpg)
### 4.之后安装FreePBX按照教程进行即可。
```
需要注意：执行到./install -n会缺少一个php -m | grep pdo_mysql
```
### 5.测试时出现Macro 扩展未加载的情况。
![4](https://github.com/xieyangp/notes/blob/main/image/Freepbx/1721788493301.jpg)
#### 5.1.找到asterisk文件夹，打开
![4](https://github.com/xieyangp/notes/blob/main/image/Freepbx/1721788533116.jpg)
#### 5.2.输入make menuselect，进入插件选择页面，在Applications找到app_macro选中，保存退出
![4](https://github.com/xieyangp/notes/blob/main/image/Freepbx/1721788552492.jpg)

-----

### 配置Freepbx
