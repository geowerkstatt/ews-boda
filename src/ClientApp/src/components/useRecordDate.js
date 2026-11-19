import { useState } from "react";

const hasRecord = (record) => Boolean(record);

// The API sends `datum` as an ISO string, but the DatePicker only accepts Date objects.
const toPickerDate = (datum) => (datum ? new Date(datum) : null);

/**
 * Holds the DatePicker value for a record's `datum` outside of react-hook-form, so manual input keeps
 * the dd.MM.yyyy format instead of being stored as an unformatted string.
 *
 * The value is reset to `record.datum` whenever a different record object is passed in, as long as
 * `shouldReset(record)` returns true. Otherwise the value the user picked is kept.
 */
export function useRecordDate(record, shouldReset = hasRecord) {
  const [selectedDate, setSelectedDate] = useState(() => (shouldReset(record) ? toPickerDate(record.datum) : null));
  const [shownRecord, setShownRecord] = useState(record);

  if (record !== shownRecord) {
    setShownRecord(record);
    if (shouldReset(record)) {
      setSelectedDate(toPickerDate(record.datum));
    }
  }

  return [selectedDate, setSelectedDate];
}
