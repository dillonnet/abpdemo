## 开发环境配置

- 进入ci,运行下面命令,docker会启动postgres和redis容器
```
docker-compose -f ./docker-compose-dev.yml  --env-file ./.env.dev up  -d
```