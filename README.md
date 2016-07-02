# System.Web.Bem

System.Web.Bem - БЭМ-инфрастурктура для ASP.NET MVC.

## Быстрый старт

1. Установите [пакет System.Web.Bem](https://www.nuget.org/packages/System.Web.Bem/) в свой проект ASP.NET MVC.
  ```bash
  PM> Install-Package System.Web.Bem -Pre
  ```

1. Верните из метода контроллера экземпляр класса `BemhtmlResult`, передав ему в конструктор нужный bemjson.
  ```cs
  using System.Web.Bem;
  ...
  public class DefaultController : Controller
  {
    public ActionResult Index()
    {
      return new BemhtmlResult(new { block = "p-index" });
    }
  }
  ```

1. Если нужно внутри Razor-шаблона вставить БЭМ-блок, используйте хелпер `@Html.Bem`, передав ему нужный bemjson.
  ```cs
  @Html.Bem(new { block = "my-block", data = Model })
  ```

## Подробнее

### Специфика БЭМ-проектов
[БЭМ](https://bem.info) (Блок-Элемент-Модификатор) - это, придуманная в [Яндексе](https://yandex.ru), методология разработки веб-приложений, в основе которой лежит компонентный подход. БЭМ - это также набор инструментов для удобной разработки в соответствии с принципами методологии. БЭМ помогает быстрее разрабатывать сайты и поддерживать их долгое время.

Согласно правилам БЭМ, приложение состоит из независимых блоков, которые лежат в отдельных папках. Каждый блок реализован в нескольких технологиях (шаблоны, стили, клиентский код). Чтобы код блоков мог работать в приложении, блоки собирают в бандлы.

Декларация бандла - файл с перечислением блоков, которые должны попасть в бандл. На основе декларации сборщик собирает бандл, учитывая зависимости блоков и уровни переопределения. Бандл собирается отдельно для каждой технологии. Во время работы приложения бандл шаблонов используется для формирования html (на сервере и клиенте), бандлы js и css подключаются на страницы и используются на клиенте. 

[BEMHTML](https://github.com/bem/bem-xjst) - специальный шаблонизатор, который удобно использовать в БЭМ-проектах.

**System.Web.Bem** - БЭМ-инфрастурктура для ASP.NET MVC проектов. При установке в проект [NuGet пакета System.Web.Bem](https://www.nuget.org/packages/System.Web.Bem/):
- из npm ставится сборщик [enb](https://ru.bem.info/toolbox/enb/) и нужные для сборки enb-модули;
- добавляется папка `Bem` с файловой структурой БЭМ проекта и настройками для сборки с помощью enb;
- добавляется дополнительный этап сборки: запуск enb - таким образом при компиляции C# в Visual Studio выполняется также и сборка БЭМ-бандлов;
- подключается C# библиотека `System.Web.Bem` для серверной шаблонизации во время работы приложения.

### Структура проекта

```
<Project root>                  // корневая папка проекта
├─ Bem                          // папка 
│  ├─ .enb
│  │  └─ make.js                // конфиг сборщика enb
│  ├─ desktop.blocks            // уровень переопределения, внутри находятся блоки
│  │  ├─ block-1 
│  │  ├─ block-2 
│  │  │  ... 
│  │  └─ block-n 
│  │     ├─ block-n.bemhtml.js  // реализация блока block-n в технологии bemhtml.js
│  │     ├─ block-n.css         // реализация блока block-n в технологии css 
│  │     │  ...                 // ...
│  │     └─ block-n.js
│  └─ desktop.bundles           // папка с бандлами проекта
│     ├─ bundle-1 
│     ├─ bundle-2 
│     │  ... 
│     └─ bundle-n 
│        └─ bundle-n.bemdecl.js // декларация бандла bundle-n 
│  ...
├─ Controllers                  // Controllers, Models, Views - стандартные папки ASP.NET MVC
├─ Models
├─ Views
│  ...
├─ package.json                 // конфиг npm
└─ Web.config                   // конфиг вашего приложения
```

### Сборка
Чтобы код блоков мог работать в приложении, блоки собирают в бандлы. Сборка бандла выполняется на основе декларации - специального файла с расширением `bemdecl.js`, где перечислены блоки, которые должны попасть в бандл. Пример декларации бандла:

```javascript
exports.blocks = [
  { name: 'block1' },
  { name: 'block2' }
];
```
Декларации находятся в папке `/Bem/desktop.bundles`, каждый бандл в своей папке. Например, бандл "default" должен находиться в папке `/Bem/desktop.bundles/default/default.bemdecl.js`. Во время сборки ищутся все декларации внутри `/Bem/desktop.bundles` и для каждой из них собираются бандлы технологий (шаблоны, js, css). Бандлы технологий имеют имя `<bundle_name>.<tech_ext>` и сохраняются в папку, где находится декларация. Например, файл шаблонов (bemhtml.js) для бандла "default" будет иметь путь `/Bem/desktop.bundles/default/default.html.js`.

### Серверная шаблонизация

Возможны 3 варианта выбора бандла для шаблонизации ответа на http запрос:
1. один бандл на всё приложение - его название можно задать в параметре `DefaultBundle` (по умолчанию `default`)
```xml
<bemSettings Mapper="Single" DefaultBundle="index" />
```
1. отдельный бандл для каждого серверного контроллера
```xml
<bemSettings Mapper="ByController" />
```
  Название бандла определяется по названию контроллера: слова разделяются дефисами, приводятся к нижнему регистру, удаляется суффикс "controller" и добавляется префикс `p-` (например, `MainPageController` → `p-main-page`). Есть идея еще добавить возможность настройки названия бандла для контроллера через C# атрибуты.
1. кастомный мэппер - есть возможность написать свой класс мэппера и указать его название в параметре `Mapper`:
```xml
<bemSettings Mapper="MyApplication.MyNamespace.InnerNamespace.MyMapperClass" />
```
Класс мэппера [должен быть унаследован](https://github.com/dima117/bemtest-net/blob/master/System.Web.Bem/BundleMappers/Mapper.cs) от `System.Web.Bem.BundleMappers.Mapper` и реализовывать метод `abstract string GetBundleName(ControllerContext context)`, получающий на вход контекст запроса и возвращающий название бандла. Также, при желании, можно переопределить метод `virtual string GetBundlePath(string bundleName)`, возвращающий по названию бандла путь к файлу с bemhtml шаблонами (по умолчанию формируется путь `<RootDir>\<bundleName>\<bundleName>.bemhtml.js`)

### Подключение библиотек с блоками

...

## Публикации
- https://ru.bem.info/forum/1007/
- https://ru.bem.info/forum/1048/
