import { useRef, useState } from "react";

const errorTexts = {
  invalidDate: "Geben Sie ein gültiges Datum im Format TT.MM.JJJJ ein",
  disableFuture: "Das Datum darf nicht in der Zukunft liegen",
};
const fallbackErrorText = "Geben Sie ein gültiges Datum ein";
const rejectedPasteText = "Das eingefügte Datum muss im Format TT.MM.JJJJ sein";

/**
 * Guards a MUI X DatePicker against saving input it could not read.
 *
 * An incomplete, impossible or out of range input reaches `onChange` as an invalid date and is reported
 * through `onError`; `hasError` is true until the input is corrected, so the form can block saving.
 *
 * Pasting text that does not match the format makes MUI X v6 call `onChange(null)` while its field keeps
 * showing the previous date. Such a paste is rejected: the change is not passed on and the picker is
 * remounted through `key`, so it shows the kept value again. Clearing the field still passes `null` on
 * and is valid, as it removes the date.
 *
 * Use `key` and spread `pickerProps` on the DatePicker, spread `textFieldProps` on its `textField` slot.
 */
export function useDatePickerValidation(onChange) {
  const [error, setError] = useState(null);
  const [rejectedPasteCount, setRejectedPasteCount] = useState(0);
  const [isPasteRejected, setIsPasteRejected] = useState(false);
  const isPasting = useRef(false);
  const hasError = error !== null;

  const handlePaste = () => {
    // MUI handles the paste synchronously right after this handler, so the flag only covers that change.
    isPasting.current = true;
    queueMicrotask(() => {
      isPasting.current = false;
    });
  };

  const handleChange = (value, context) => {
    if (value === null && isPasting.current) {
      setRejectedPasteCount((count) => count + 1);
      setIsPasteRejected(true);
      return;
    }
    setIsPasteRejected(false);
    onChange(value, context);
  };

  const helperText = () => {
    if (hasError) return errorTexts[error] ?? fallbackErrorText;
    if (isPasteRejected) return rejectedPasteText;
    return "";
  };

  return {
    hasError,
    key: rejectedPasteCount,
    pickerProps: {
      onChange: handleChange,
      onError: (newError) => setError(newError),
    },
    textFieldProps: {
      helperText: helperText(),
      onPaste: handlePaste,
    },
  };
}
