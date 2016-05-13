# dsdg
## Вступление
Я давно хотел использовать БЭМ в  .NET проектах.
## Реализация
Нужно сделать сборку и шаблонизацию во время работы приложения.
### Сборка
Простой путь: msbuild - сборщик проектов на .NET, он может запускать консольные приложения. файл проекта - по сути, конфиг msbuild, можно ручками добавить туда вызов `enb make`. Достоинства: это просто сделать + сборка на enb хорошо работает. Недостатки: нет интеграции с UI => неудобно настраивать и использовать.
Продвинутый вариант: сборка на gulp. В последних версиях Visual Studio есть интеграция с gulp + на последнем хакатоне по БЭМ уже был более-менее рабочий пример сборки БЭМ проектов на gulp. Достоинства: интеграция с UI, gulp гибче и удобнее настраивается, чем enb, работает на основе менее мозговыносящей идеи и более распространен.

### Что получилось
Файловая структура проекта:
```
<Project>
├─ Bem
    ├─ desktop.blocks
        ├─ block1
        └─ block2
    └─ desktop.bundles
        ├─ bundle1
            └─ bundle.bemjson.js
        └─ bundle2
├─ Controllers
  ...
└─ gulpfile.js
```

Сборка шаблонов
```
gulp.task('bemhtml', function() {
    return bundle.src({ tech: 'bemhtml', extensions: ['.bemhtml', '.bemhtml.js'] })
        .pipe(concat(bundle.name() + '.bemhtml.js'))
        .pipe(through2(function(file, encoding, callback) {
            var src = file.contents.toString(encoding),
                bundle = bemhtml.generate(src);
            file.contents = new Buffer(bundle, encoding);
            callback(null, file);
        }))
        .pipe(gulp.dest('Bem/desktop.bundles/index'));
});
```

особенности:
- вся магия в `bundle.src` - это как gulp.src, только возвращает файлы из всех уровней переопределения в правильном порядке
- если нет файлов deps, то сборка падает
- мне не удалось запустить из VS node с поддержкой ES6, поэтому не смог воспользоваться нормальным плагином для сборки bemhtml и написал руками

Сборка js и стилей - аналогично. Весь код можно посмотреть [здесь](https://github.com/dima117/bemtest-net/blob/master/WebApplication/gulpfile.js): 

Таким образом, получился проект ASP.NET MVC, с которым мы работаем через Visual Studio. В проекте есть папка с БЭМ блоками и при сборке проекта через VC вместе с компиляцией кода на C# происходит сборка БЭМ-бандлов: серверные шаблоны, клиентский js и стили.