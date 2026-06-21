#!/bin/bash

echo "============================================"
echo "     Project Report Generator"
echo "============================================"
echo ""

read -rp "Введите путь к папке проекта: " PROJECT_PATH

if [ ! -d "$PROJECT_PATH" ]; then
    echo ""
    echo "ОШИБКА: Папка \"$PROJECT_PATH\" не найдена."
    echo "Проверьте путь и запустите скрипт снова."
    echo ""
    exit 1
fi

echo ""
echo "Папка найдена. Создаём отчёт..."

mkdir -p "$PROJECT_PATH/reports"

REPORT_FILE="$PROJECT_PATH/reports/report.txt"

{
echo "============================================"
echo "          ОТЧЁТ О ПРОЕКТЕ"
echo "============================================"
echo ""
echo "Дата и время создания отчёта: $(date '+%d.%m.%Y %H:%M:%S')"
echo "Пользователь:                 $USER"
echo "Имя компьютера:               $HOSTNAME"
echo "Путь к папке проекта:         $PROJECT_PATH"
echo ""
echo "---- СПИСОК ВСЕХ ФАЙЛОВ И ПАПОК ----"
echo ""
ls -la "$PROJECT_PATH"
echo ""

echo "---- КОЛИЧЕСТВО ФАЙЛОВ ----"
FILE_COUNT=$(find "$PROJECT_PATH" -maxdepth 1 -type f | wc -l)
echo "Файлов в корне папки: $FILE_COUNT"
echo ""

echo "---- КОЛИЧЕСТВО ПАПОК ----"
DIR_COUNT=$(find "$PROJECT_PATH" -maxdepth 1 -type d | tail -n +2 | wc -l)
echo "Папок в корне папки: $DIR_COUNT"
echo ""

echo "---- ФАЙЛЫ ПО РАСШИРЕНИЯМ (.py .html .css .js .txt) ----"
echo ""

echo "[.py — Python файлы]"
find "$PROJECT_PATH" -name "*.py" | while read -r f; do echo "  $f"; done
echo ""

echo "[.html — HTML файлы]"
find "$PROJECT_PATH" -name "*.html" | while read -r f; do echo "  $f"; done
echo ""

echo "[.css — CSS файлы]"
find "$PROJECT_PATH" -name "*.css" | while read -r f; do echo "  $f"; done
echo ""

echo "[.js — JavaScript файлы]"
find "$PROJECT_PATH" -name "*.js" | while read -r f; do echo "  $f"; done
echo ""

echo "[.txt — текстовые файлы]"
find "$PROJECT_PATH" -name "*.txt" | while read -r f; do echo "  $f"; done
echo ""

echo "============================================"
echo "           КОНЕЦ ОТЧЁТА"
echo "============================================"
} > "$REPORT_FILE"

echo ""
echo "Отчёт успешно создан: $REPORT_FILE"
echo ""
