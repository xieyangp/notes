Sourcetree的使用
====
####  一、 从github复制需要的项目HTTPS地址
![4](https://github.com/xieyangp/notes/blob/main/image/sourcetree/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-11-11%20151631.png) 

####  二、 打开sourcetree的clone将复制的HTTPS粘贴到源地址，建立本地库
![4](https://github.com/xieyangp/notes/blob/main/image/sourcetree/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-11-11%20152255.png) 
####  三、在本地建立空的文件夹放Clone的项目，浏览新建的文件夹，将地址复制或通过浏览找到文件地址

####  四、点击克隆，克隆完后打开文夹，选择开发工具将其打开即可

####  五、在新建分支时，我们需要在哪个主分支的基础上新建分支必须先要切换到对应的主分支才能到该主分支上创建分支，如下我们要在master分支上创建一个autofac新建分支：
![4](https://github.com/xieyangp/notes/blob/main/image/sourcetree/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-11-11%20170536.png)      
 注意：如果需要切换到其他分支，可以直接双击对应的分支切换到该分支，前提是需要先从远程检出该分支；
 
####  六、合并分支，比如选取master分支，右击autofac，选择合并autofac分支至当前分支即可进行合并，在合并代码之前我们都需要将需要合并的分支拉取到最新状态避免覆盖别人的代码，或者代码丢失：
![4](https://github.com/xieyangp/notes/blob/main/image/sourcetree/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-11-12%20131321.png)      

####   这里需要明确几个概念之间的区别：
      1、提交和推送：提交只是将暂存区的文件上传到我们本地的代码库，而推送则是将本地仓库同步至远程仓库，这样操作之后别人才能从远程拉取你修改的最新代码。
    
      2、拉取和获取：拉取(pull)是从远程仓库获取信息并同步至本地仓库，并且自动执行合并（merge）操作（git pull=git fetch+git merge）。而获取(fetch)则只是从远程仓库获取信息并同步至本地仓库。所以一般推送之前需要先拉取一次，确保代码一致。
    
      3、丢弃和移除：丢弃指的是丢弃更改,恢复文件改动/重置所有改动,即将已暂存的文件丢回未暂存的文件。移除则是移除文件至缓存区。
      
####   七、提交修改代码，修改过的文件将会被存储在暂存区,在文件状态可以查看修改的文件，将文件放入已暂存文件区点击提交，本地仓库就会被更新，在推送至远程仓库即可更新远程仓库
![4](https://github.com/xieyangp/notes/blob/main/image/sourcetree/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-11-12%20132034.png)    

####   八、如果发现一个文件修改错了，那么可以右键这个文件，选择丢弃，将该文件的所有修改丢弃，回滚到你修改之前的状态：
![4](https://github.com/xieyangp/notes/blob/main/image/sourcetree/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-11-12%20153115.png)    
