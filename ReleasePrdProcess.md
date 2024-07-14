# 发布prd流程
## Github流程
## 第一步整理Milestones(里程碑)：
### 1、开github lssues 找到Milestones 点击进去；
### 2、点击 New Milestone，创建一个新的milestone；
### 3、将要打包的Milestone中的未完成的pr转移到新的Milestone，确保即将打包的Milestone的完成度为100%；
### 4、然后关闭即将打包的Milestone；
## 第二步发布Release
### 5、打开Releases，进行发布流程;
### 6、点击Draft a new release 创建一个新的release；
### 7、点击Choose a tag选择一个标签，这里与要发布Milestone的一样，然后点击Create new tag；
### 8、Release title也与Milestone一样
### 9、点击publish release即好
## Teamcity流程
## 观察是否有构建刚刚发布release即好，如果没有马上看到可以点击run构建一次
## output流程
