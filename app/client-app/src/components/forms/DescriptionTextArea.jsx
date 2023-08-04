import { useField } from "formik";
import { useDebounce } from "helpers/hooks/useDebounce";
import { TextArea } from "./TextArea";
import { useState, useEffect } from "react";

export const DescriptionTextArea = ({
  name,
  title,
  wordLimit,
  placeholder,
}) => {
  const [field, meta, helpers] = useField({ name, type: "text" });

  const [value, setValue] = useState(field.value);
  const debouncedValue = useDebounce(value, 150);

  useEffect(() => {
    helpers.setValue(debouncedValue);
  }, [debouncedValue]);

  return (
    <TextArea
      title={title}
      wordLimit={wordLimit}
      value={value}
      placeholder={placeholder}
      onChange={setValue}
      isInvalid={meta.error && meta.touched}
      error={meta.error}
    />
  );
};
