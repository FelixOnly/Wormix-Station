# Информация о команде
roll-command-description = Бросает кости в приватном режиме или показывает результат ближайшим игрокам.
roll-command-help = Использование: roll <грани|КdГ[+/-модификатор]> [show]

# Ошибки
roll-command-invalid = Некорректный бросок. Использование: roll <грани|КдГ[+/-модификатор]> [show]
roll-command-missing-expression = Укажите, что бросить. Например: roll 2d20+5
roll-command-too-many-arguments = Слишком много аргументов. После выражения для броска может быть указано только "show"
roll-command-invalid-option = Некорректная опция. Используйте "show", чтобы показать результат ближайшим игрокам
roll-command-invalid-count = Количество костей должно быть целым числом перед "d"
roll-command-count-out-of-range = Количество костей должно быть от 1 до {$maximum}
roll-command-invalid-sides = Количество граней должно быть целым числом, например 20 или 2d20
roll-command-sides-out-of-range = Кость должна иметь от 1 до {$maximum} граней
roll-command-invalid-modifier = Модификатор должен быть целым числом с префиксом + или -
roll-command-modifier-out-of-range = Модификатор должен быть от -{$maximum} до +{$maximum}
roll-command-show-disabled = Отображение бросков ближайшим игрокам отключено. Результат броска будет показан только вам

# Результаты
roll-command-result-private = Вы выбросили {$total} ({$calculation})
roll-command-result-public = выбросил {$total} ({$calculation})