import {
  RadioGroup,
  Box,
  HStack,
  Radio,
  VStack,
  FormControl,
} from "@chakra-ui/react";
import { findKeyByPropertyValue } from "helpers/data-structures";
import { useDebounce } from "helpers/hooks/useDebounce";
import { useFieldAsObject } from "helpers/hooks/useFieldAsObject";
import { useCallback, useEffect, useMemo, useState } from "react";
import { FormHelpError } from "./FormHelpError";
import { Question } from "./Question";
import { QuestionTextarea } from "./QuestionTextarea";

export const FormikConditionalRadio = ({ radioName, textareaName, ...p }) => {
  const radioFormik = useFieldAsObject({ name: radioName });

  const textareaFormik = useFieldAsObject({ name: textareaName });

  const [textValue, setTextValue] = useState(textareaFormik.field.value);
  const debouncedValue = useDebounce(textValue, 150);

  useEffect(() => {
    textareaFormik.helpers.setValue(debouncedValue);
  }, [debouncedValue]);

  const handleRadioChange = (radioValue) =>
    radioFormik.helpers.setValue(radioValue);

  return (
    <>
      <ConditionalRadio
        {...p}
        radioValue={radioFormik.field.value}
        onRadioChange={handleRadioChange}
        textValue={textValue}
        onTextChange={setTextValue}
        radioError={radioFormik.meta.error}
        textError={textareaFormik.meta.error}
        textIsInvalid={textareaFormik.meta.error && textareaFormik.meta.touched}
      />
    </>
  );
};

const RadioButtons = ({ radioKeys, currentKey, onChange, error }) => {
  // Chakra's <Radio /> component uses `position: absolute`
  // on the actual <input /> element
  // but this breaks overflowing grid layouts, like we have
  // so we use a ref to override the position directly on the element
  const fixRadioRef = useCallback((radio) => {
    if (radio) radio.style.position = "inherit";
  });

  return (
    <FormControl>
      <RadioGroup value={currentKey} onChange={onChange}>
        <HStack pl={8} spacing={4}>
          {radioKeys.map((k) => (
            <Radio key={k} value={k} ref={fixRadioRef}>
              {k}
            </Radio>
          ))}
        </HStack>
      </RadioGroup>

      <FormHelpError
        isInvalid={error}
        error={error}
        collapseEmpty
        replaceHelpWithError
      />
    </FormControl>
  );
};

const KeyedTextarea = ({
  valueKey,
  value,
  onChange,
  error,
  isInvalid,
  wordLimit,
  children,
}) => {
  const [values, setValues] = useState({ [valueKey]: value ?? "" });

  const handleChange = (v) => {
    setValues((current) => ({ ...current, [valueKey]: v }));
    onChange(v);
  };

  useEffect(() => {
    setValues((current) => ({ ...current, [valueKey]: value }));
  }, [value]);

  useEffect(() => {
    onChange(values[valueKey]);
  }, [valueKey]);

  return (
    <Box w="100%">
      <QuestionTextarea
        isInvalid={isInvalid}
        error={error}
        wordLimit={wordLimit}
        value={values[valueKey]}
        onChange={handleChange}
      >
        {children}
      </QuestionTextarea>
    </Box>
  );
};

export const ConditionalRadio = ({
  children,
  options,
  questionNumber,

  radioValue,
  onRadioChange,
  radioError,

  textValue,
  onTextChange,
  textError,
  textIsInvalid,
}) => {
  const radioKey = useMemo(() => {
    return findKeyByPropertyValue(options, "value", radioValue) ?? radioValue;
  }, [radioValue]);

  const { question, wordLimit } = options[radioKey] ?? {};

  const handleRadioChange = (key) => {
    onRadioChange(options[key].value ?? key);
  };

  return (
    <VStack w="100%" spacing={4} align="start">
      {children && <Question number={questionNumber}>{children}</Question>}

      <RadioButtons
        radioKeys={Object.keys(options)}
        currentKey={radioKey}
        error={radioError}
        onChange={handleRadioChange}
      />

      {question && (
        <KeyedTextarea
          valueKey={radioKey}
          error={textError}
          isInvalid={textIsInvalid}
          value={textValue}
          onChange={onTextChange}
          wordLimit={wordLimit}
        >
          {question}
        </KeyedTextarea>
      )}
    </VStack>
  );
};
