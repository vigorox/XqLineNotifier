# XqLineNotifier

1. 常駐執行 XqLineNotifier.exe ，於UI中輸入Line token (以下均假設XqLineNotifier.exe放置於C:\XqLog\ 下長駐執行)
2. XQ執行Print時，必需以 ***\*.xqlog*** 的檔案名稱，輸出到 C:\XqLog資料夾中。
例如: 若是長駐執行 "C:\XqLog\XqLineNotifier.exe"，則XS中要使用以下語法:
```
Print(File("C:\XqLog\xxx.xqlog"), "[要給Line的訊息]");
```

## 限制
* 需長駐執行XqLineNotifier.exe，不可關閉
* XQ的一般Print指令的log檔案，亦會輸出到C:\XqLog下，而非原來預設的路徑

## 優點
* 傳給Line的訊息是輸出到*.xqlog，可以跟一般Print指令輸出的*.log檔案作區分