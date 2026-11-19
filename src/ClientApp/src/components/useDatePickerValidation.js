import { useRef, useState } from "react";

const errorTexts = {
  invalidDate: "Geben Sie ein gültiges Datum im Format TT.MM.JJJJ ein",
  disableFuture: "Das Datum darf nicht in der Zukunft liegen",
};
const fallbackErrorText = "Geben Sie ein gültiges Datum ein";
const rejectedPasteText = "Das eingefügte Datum muss im Format TT.MM.JJJJ sein";

const isPartiallyFilled = (sections) =>
  sections.some((section) => section.value !== "") && sections.some((section) => section.value === "");

/**
 * Guards a MUI X DatePicker against saving input it could not read.
 *
 * An incomplete, impossible or out of range input reaches `onChange` as an invalid date and is reported
 * through `onError`; `hasError` is true until the input is corrected, so the form can block saving.
 * Typing into an empty field is the exception: MUI X keeps the value `null` until every section is
 * filled, so the sections are read after each key press and a partly filled field counts as an error.
 *
 * Pasting text that does not match the format makes MUI X call `onChange(null)`, which would silently
 * remove the date. Such a paste is rejected: the change is not passed on and the picker is remounted
 * through `key`, so its internal state shows the kept value again. Clearing the field still passes
 * `null` on and is valid, as it removes the date.
 *
 * Use `key` and spread `pickerProps` on the DatePicker, pass `fieldProps` as its `field` slot props and
 * spread `textFieldProps` on its `textField` slot.
 */
export function useDatePickerValidation(value, onChange) {
  const [error, setError] = useState(null);
  const [rejectedPasteCount, setRejectedPasteCount] = useState(0);
  const [isPasteRejected, setIsPasteRejected] = useState(false);
  const [hasPartialInput, setHasPartialInput] = useState(false);
  const [checkedValue, setCheckedValue] = useState(value);
  const isPasting = useRef(false);
  const fieldRef = useRef(null);

  if (value !== checkedValue) {
    setCheckedValue(value);
    setHasPartialInput(false);
  }

  const hasError = error !== null || hasPartialInput;

  const handlePaste = () => {
    // MUI handles the paste synchronously right after this handler, so the flag only covers that change.
    isPasting.current = true;
    queueMicrotask(() => {
      isPasting.current = false;
    });
  };

  const handleKeyUp = () => {
    setHasPartialInput(isPartiallyFilled(fieldRef.current?.getSections() ?? []));
  };

  const handleChange = (newValue, context) => {
    if (newValue === null && isPasting.current) {
      setRejectedPasteCount((count) => count + 1);
      setIsPasteRejected(true);
      return;
    }
    setIsPasteRejected(false);
    onChange(newValue, context);
  };

  const helperText = () => {
    if (error !== null) return errorTexts[error] ?? fallbackErrorText;
    if (hasPartialInput) return errorTexts.invalidDate;
    if (isPasteRejected) return rejectedPasteText;
    return "";
  };

  return {
    hasError,
    key: rejectedPasteCount,
    pickerProps: {
      value,
      onChange: handleChange,
      onError: (newError) => setError(newError),
    },
    fieldProps: { fieldRef },
    textFieldProps: {
      error: hasError,
      helperText: helperText(),
      onPaste: handlePaste,
      onKeyUp: handleKeyUp,
    },
  };
}
