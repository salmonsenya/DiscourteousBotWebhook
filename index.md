Это краткое руководство-чеклист я пишу для самой себя, чтобы систематизировать нагугленную информацию по процессу создания Telegram бота, и не тратить время на поиск полезных ссылок, когда я вернусь к ботам в следующий раз. Гайд вероятно получится, во-первых, очень нубовским, во-вторых, узконаправленным, начиная от выбора конкретной библиотеки для работы с *Telegram Bot API*, и заканчивая выбором сервера. И в-третьих, это скорее чеклист, так как я не вижу смысла в копипасте документации, по возможности я оставлю ссылки.

Лучше начать с покупки домена и сервера, так как регистрация, выпуск сертификата и обновление DNS-серверов может занять какое-то время.
# Сервер Digital Ocean
Мой бот хостится на облачной платформе [Digital Ocean](https://www.digitalocean.com/). Выбор был обусловлен хорошими отзывами и тем, что у них есть промокоды, которых хватает на несколь месяцев бесплатного использования. Купоны на $10-$100 легко гуглятся, например, [здесь](https://www.newcoupons.info/digitalocean-coupon-codes/).

## Создайте новый Droplet
* Войдите (создайте) в свой аккаунт на Digital Ocean
* Зайдите во вкладку *Billing -> Promo code* и вставьте код. (1 промокод на 1 аккаунт)
* Создайте новый *Droplet*: Подробная инструкция по созданию *droplet*'а [Droplet Quickstart](https://www.digitalocean.com/docs/droplets/quickstart/).
    * Выберите Ubuntu 20.04
    * План Basic ($5/месяц)
    * Authentication == создание *root* пароля (SSH добавим позже)
    * Остальные настройки по умолчанию
    
## Выполните начальную настройку сервера
Зайдите в *Droplet* через консоль (на Windows можно использовать Windows PowerShell) под *root* пользователем, создайте нового пользователя и предоставьте ему административные права. [Гайд по начальной настройке сервера Ubuntu 20.04](https://www.digitalocean.com/community/tutorials/initial-server-setup-with-ubuntu-20-04-ru).

## Установка .NET Core SDK на Ubuntu
#### Регистрация ключа Microsoft
Перед установкой .NET вам необходимо зарегистрировать ключ Microsoft, зарегистрировать репозиторий Microsoft и установить необходимые зависимости.
Откройте командную строку и выполните следующие команды:
```
wget -q https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
```

#### Установка .NET Core SDK
Необходимо обновить продукты, доступные для установки, а затем установить .NET SDK. В командной строке выполните следующие команды:
```
sudo apt-get install apt-transport-https
sudo apt-get update
sudo apt-get install dotnet-sdk-3.1
```
Проверьте правильность установки:
```
dotnet --version
```

## Установка Nginx на Ubuntu
Мы будем использовать ```apt-get``` для установки Nginx. Установщик создаст скрипт инициализации *systemd*, который будет запускать Nginx как демон при каждом запуске системы. Выполним следующую команду:
```
sudo -s
nginx=stable # use nginx=development for latest development version
add-apt-repository ppa:nginx/$nginx
apt-get update
apt-get install nginx
```
Поскольку Nginx был установлен впервые, мы должны явно запустить его, выполнив:
```sudo service nginx start```
Мы можем проверить, отображает ли браузер страницу по умолчанию для Nginx. Целевая страница доступна по адресу ```http://<server_IP_address>```


# Доменное имя
## Покупка домена
Приобретаем доменное имя у одного из аккредитированных регистраторов. Например, я купила домен на [REG.RU](https://www.reg.ru/), в дальнейшем речь пойдёт о нём.

## Добавление домена на сервер
Зайдите в панель управления Digital Ocean, во вкладке *Networking* [добавьте домен](https://www.digitalocean.com/docs/networking/dns/how-to/add-domains/).
Проверьте, что у домена существуют следующие DNS-записи и при необходимости добавьте:

| Type          | Hostname              | Value                       |
| ------------- | --------------------- | --------------------------- |
| A             | ```www.<domen_name>```| ```<server_IP_address>```   |
| A             | ```<domen_name>```    | ```<server_IP_address>```   |
| NS            | ```<domen_name>```    | ```ns1.digitalocean.com.``` |
| NS            | ```<domen_name>```    | ```ns2.digitalocean.com.``` |
| NS            | ```<domen_name>```    | ```ns3.digitalocean.com.``` |


Зайдите в панель управления регистратора вашего домена и измените существующие по умолчанию DNS-серверы на серверы Digital Ocean. Новый список серверов должен быть следующим:  
```
ns1.digitalocean.com
ns2.digitalocean.com
ns3.digitalocean.com
```

## Покупка SSL-сертификата
У того же регистратора заказываем SSL-сертификат на актуальное доменное имя. В данный момент SSL-сертификат уровня DV на 1 год при регистрации домена на REG.RU можно заказать бесплатно.

## Активация SSL-сертификата
Следующий этап манипуляций с сертификатом после покупки - его активация. Он состоит из двух пунктов:
1. Скопировать TXT-запись из письма, которое должно прийти после покупки сертификата.
2. Добавить TXT-запись на ваш сервер: зайдите в панель управления Digital Ocean, зайтдите во вкладку *Domains*, выберите свой домен, и для него создайте новую TXT-запись.
3. Готово. После активации SSL на контактную почту будет выслано письмо с данными для установки SSL-сертификата.
Подробнее про активацию SSL-сертификата на REG.RU [тут](https://www.reg.ru/support/ssl-sertifikaty/aktivatsiya-ssl-sertifikata/kak-dobavit-zapis-txt-dlya-besplatnogo-ssl-sertifikata).
4. После активации можно удалить TXT-запись с сервера.

# Бот на ASP.NET Core
После того как основная пренастройка сервера и домена выполнена, вернемся к самому боту.

## Создание бота
Выполните начальные шаги из гайда *Telegram Bot Api* и получите *token* своего бота с помощью BotFather. Осталось написать код.

## Создание ASP.NET приложения
Базовый бот в стиле MVC легко пишется, если воспользоваться .NET клиентом для работы с *Telegram Bot API* [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot). У него есть документация, гайд и примеры. Добавьте *token*, полученный от BotFather в конфиги, например, в *appsettings.json*.
Убедитесь, что бот работает локально и отвечает на ```http://localhost:5000```, ```https://localhost:5001```.  Отправьте боту *update*'ы (например через Postman). Примеры *json*-объектов *update*'ов можно найти [здесь](https://core.telegram.org/bots/webhooks#testing-your-bot-with-updates).

Сделайте *publish* проекта с ботом в локальную папку. Зайдите в эту папку в консоли и запустите приложение ```dotnet <your_app_name>.dll```, убедитесь что оно также работает.

## Публикация бота на сервере с помощью FileZilla
#### Настройка SSH
Чтобы передать файлы проекта на сервер по SFTP (SSH File Transfer Protocol), воспользуемся FileZilla - бесплатным FTP-клиентом.
Для этого сначала создадим SSH-ключи, добавим их к нашему *Droplet*'у и попробуем подключиться к удаленному серверу через SSH-соединению. [Клик как это сделать](https://www.digitalocean.com/docs/droplets/how-to/add-ssh-keys/).

#### FileZilla
После того, как мы убедились в том, что SSH-соединение работает, с помощью FileZilla передадим папку с *publish* проекта на удаленный сервер [(см.инструкцию)](https://www.digitalocean.com/docs/droplets/how-to/transfer-files/).

# Запуск бота на сервере
## Установка SSL-сертификата на Nginx
В результате следования [этому руководству](https://www.reg.ru/support/ssl-sertifikaty/ustanovka-ssl-sertifikata/ustanovka-ssl-sertifikata-na-nginx) в директории ```/etc/ssl/``` сервера появятся файлы сертификата и ключа ```<ssl_certificate_name>.crt```, ```<ssl_key_name>.key```.

## Конфигурация Nginx
Чтобы настроить Nginx в качестве обратного прокси-сервера для пересылки запросов в наше приложение ASP.NET Core, нам необходимо изменить файл ```/etc/nginx/sites-available/default```. Отредактируйте его так, чтобы он соответствовах привиденным ниже конфигам. Обратите внимание на то, что информация о порте ssl, ssl-сертификате и ключе, которую вы добавили в конфигурацию Nginx в предыдущем пункте, не должна затереться.
```
server {
    listen 80;
    listen 443 ssl;
    ssl_certificate /etc/ssl/<ssl_certificate_name>.crt;
    ssl_certificate_key /etc/ssl/<ssl_key_name>.key;
    server_name <your_domen_name>;
    location / {
        proxy_pass https://localhost:5001;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}
```

После того, как конфигурация Nginx изменена, мы должны запустить ```sudo nginx -t```, чтобы проверить синтаксис файлов конфигурации. Если проверка файла конфигурации прошла успешно, нужно заставить Nginx применить изменения в конфигурации, запустив ```sudo nginx -s reload```.

## Запуск бота как сервиса
Мы должны использовать *systemd* для создания служебного файла для запуска и мониторинга базового веб-приложения. Создадим служебный файл:
```sudo nano /etc/systemd/system/<your_service_name>.service```
Затем, внутри этого файла добавляем следующие настройки конфигурации:
```
[Unit]
Description=Pizdabot .NET Core Telegram Bot on Ubuntu

[Service]
WorkingDirectory=/root/discourteous
ExecStart=/usr/bin/dotnet /root/discourteous/DiscourteousBotWebhook.dll
Restart=always
RestartSec=10
SyslogIdentifier=offershare-web-app
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
```

Теперь сделаем сервис доступным и запустим его:
```
 sudo systemctl enable <your_service_name>.service
 sudo systemctl start <your_service_name>.service
 sudo systemctl status <your_service_name>.service
```
Команда *status* должна показать, что служба работает (если конфигурация правильная). Итак, наше веб-приложение запущено. Мы успешно разместили бот под Linux с помощью Nginx.


# Настройка Webhook
Подробный гайд по настройке *Webhook* можно прочитать [здесь](https://core.telegram.org/bots/webhooks#testing-your-bot-with-updates). По сути, всё, что достаточно сделать, - это сообщить *Telegram*'у адрес метода бота, который принимает *update*'ы и обрабатывает их. Сделать это можно с помощью метода *setWebHook*, сделав запрос (например просто в браузере):
```https://api.telegram.org/bot{my_bot_token}/setWebhook?url={url_to_send_updates_to}```,  
где  
* ```my_bot_token``` - это токен, который мы получили от *BotFather*, когда создавали бот
* ```url_to_send_updates_to``` - *url* метода бота, обрабатывающего запросы (обязательно HTTPS)

# Готово, вы восхитительны!
