git
====
##  git从github克隆项目
####  一、先在github复制需要的项目HTTPS,即项目的远程仓库地址
![项目HTTPS](https://github.com/xieyangp/notes/blob/main/image/git/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-11-12%20160403.png)
####  二、在本地创建一个新的文件夹用来当本地仓库，打开git bash，通过cd命令选择本地文件夹
![git bash选择本地仓库](https://github.com/xieyangp/notes/blob/main/image/git/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-11-12%20182715.png)
####  三、通过git clone 远程仓库地址克隆项目至本地仓库
![git bash克隆](https://github.com/xieyangp/notes/blob/main/image/git/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-11-12%20183322.png)
####  四、当我们对项目进行修改后，通过 git add .进行跟踪,在通过git commit -m "提交信息"提交至本地仓库
![提交至本地](https://github.com/xieyangp/notes/blob/main/image/git/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-11-12%20183952.png)  
  报错信息：fatal: not a git repository (or any of the parent directories): .git  
  原因：没有选择git存储库，进行git提交等命令  
  注意：在克隆完后，要通过cd命令选择本地仓库地址，否则无法使用git进行提交等操作  
####  五、当我要推送项目至远程仓库时，最好先从远程仓库拉取最新的项目，避免覆盖
![获取项目](https://github.com/xieyangp/notes/blob/main/image/git/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-11-12%20202658.png) 
####  六、获取最新项目之后，将项目推送至远程仓库
![推送项目](https://github.com/xieyangp/notes/blob/main/image/git/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-11-12%20204142.png) 
####  七、常用的git命令
![git命令](https://github.com/xieyangp/notes/blob/main/image/git/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-11-12%20200017.png)
####  八、使用git bash时可能出现连接超时，这时候可以将代理设置一下，先设置本地服务器代理，在将设置git的代理
设置本地代理  
![设置本地代理](https://github.com/xieyangp/notes/blob/main/image/git/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-11-12%20202725.png)   
设置git代理    
git config --global http.proxy http://代理IP地址:端口号  
git config --global https.proxy https://代理IP地址:端口号  
查看git代理是否成功  
git config http.proxy  
git config https.proxy
