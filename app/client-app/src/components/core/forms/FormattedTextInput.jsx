import { useRef } from "react";
import { Box, FormLabel, FormControl, useTheme } from "@chakra-ui/react";
import ReactQuill from "react-quill";
import { useField } from "formik";
import "react-quill/dist/quill.snow.css";
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
  const theme = isDisabled ? "bubble" : "snow";
  const ui = useTheme();

  const modules = {
    toolbar: [
      ["bold", "italic", "underline", "strike"],
      [{ script: "sub" }, { script: "super" }],
    ],
  };

  const formats = ["bold", "italic", "underline", "strike", "script"];

  const quillRef = useRef(null);

  const [field, meta, helpers] = useField(name);
  const { setValue } = helpers;

  return (
    <FormControl
      id={field.name}
      isRequired={isRequired}
      isInvalid={meta.error && meta.touched}
    >
      {label && <FormLabel>{label}</FormLabel>}

      <Box
        w="full"
        borderRadius="4"
        borderWidth={1}
        sx={{
          ".ql-editor": {
            height: "250px",
            fontFamily: ui.fonts.body,
            fontSize: "sm",
          },
        }}
      >
        <ReactQuill
          ref={quillRef}
          theme={theme}
          modules={modules}
          formats={formats}
          placeholder={placeholder}
          value={field.value}
          onChange={(content, delta, source, editor) =>
            setValue(editor.getHTML())
          }
          readOnly={isDisabled}
        />
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
