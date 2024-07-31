import { useEffect, useRef } from "react";
import { Box, FormLabel, FormControl, Text } from "@chakra-ui/react";
import { useQuill } from "react-quilljs";
import { useField } from "formik";
import "quill/dist/quill.snow.css";
import { FormHelpError } from "./FormHelpError";

const FormattedTextInput = ({
  label,
  name,
  isRequired,
  placeholder,
  isDisabled,
  fieldTip,
  fieldHelp,
  collapseError,
}) => {
  const theme = "snow";

  const modules = {
    toolbar: [
      ["bold", "italic", "underline", "strike"],
      [{ script: "sub" }, { script: "super" }],
    ],
  };

  const formats = ["bold", "italic", "underline", "strike", "script"];

  const { quill, quillRef } = useQuill({
    theme,
    modules,
    formats,
    placeholder,
  });
  const initializedRef = useRef(false);

  const [field, meta, helpers] = useField(name);
  const { setValue } = helpers;

  useEffect(() => {
    if (quill && !initializedRef.current) {
      initializedRef.current = true;

      // Initialize editor with value
      quill.root.innerHTML = field.value;
      quill.enable(!isDisabled);

      // Setup change handler
      quill.on("text-change", () => {
        setValue(quill.root.innerHTML);
      });
    }
  }, [quill, field.value, isDisabled, setValue]);

  return (
    <FormControl
      id={field.name}
      isRequired={isRequired}
      isInvalid={meta.error && meta.touched}
    >
      {label && (
        <FormLabel>
          <Text as="b">{label}</Text>
        </FormLabel>
      )}

      <Box
        w="full"
        height="400px"
        borderColor="gray.300"
        borderRadius="lg"
        borderWidth={1}
        overflow="hidden"
      >
        <Box ref={quillRef} />
      </Box>

      {fieldTip}

      <FormHelpError
        isInvalid={meta.error && meta.touched}
        error={meta.error}
        help={fieldHelp}
        collapseEmpty={collapseError}
        replaceHelpWithError
      />
    </FormControl>
  );
};

export default FormattedTextInput;
