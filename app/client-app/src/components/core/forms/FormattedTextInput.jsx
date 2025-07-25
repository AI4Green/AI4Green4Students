import React, { forwardRef, useEffect, useLayoutEffect, useRef } from "react";
import { Box, FormLabel, FormControl, useTheme } from "@chakra-ui/react";
import { useField } from "formik";
import { FormHelpError } from "./FormHelpError";
import Quill from "quill";

const DEFAULT_FORMATS = ["bold", "italic", "underline", "strike", "script"];
const DEFAULT_MODULES = {
  toolbar: [
    ["bold", "italic", "underline", "strike"],
    [{ script: "sub" }, { script: "super" }],
  ],
};

export const FormattedTextInput = ({
  label,
  name,
  isRequired,
  placeholder,
  isDisabled,
  fieldTip,
  fieldHelp,
  collapseError,
}) => {
  const ui = useTheme();
  const quillRef = useRef(null);
  const [field, meta, helpers] = useField(name);
  const { setValue } = helpers;

  const modules = isDisabled ? false : { ...DEFAULT_MODULES };

  const formats = [...DEFAULT_FORMATS];

  const handleTextChange = (content, delta, source, editor) => {
    setValue(editor.getHTML());
  };

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
            overflowY: "auto",
          },
        }}
      >
        <Editor
          ref={quillRef}
          readOnly={isDisabled}
          defaultValue={field.value || ""}
          placeholder={placeholder}
          modules={modules}
          formats={formats}
          onTextChange={handleTextChange}
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

export const FormattedTextInputEditor = forwardRef(
  (
    { readOnly, defaultValue, placeholder, modules, formats, onTextChange },
    ref,
  ) => {
    const containerRef = useRef(null);
    const onTextChangeRef = useRef(onTextChange);

    useLayoutEffect(() => {
      onTextChangeRef.current = onTextChange;
    });

    useEffect(() => {
      const container = containerRef.current;
      if (!container) return;

      const editorContainer = container.appendChild(
        container.ownerDocument.createElement("div"),
      );

      const quill = new Quill(editorContainer, {
        theme: "snow",
        modules: modules || DEFAULT_MODULES,
        formats: formats || DEFAULT_FORMATS,
        placeholder: placeholder || "",
        readOnly,
      });

      quill.getHTML = () => quill.root.innerHTML;
      ref.current = quill;

      if (defaultValue) {
        if (typeof defaultValue === "string") {
          quill.clipboard.dangerouslyPasteHTML(defaultValue);
        } else {
          quill.setContents(defaultValue);
        }
      }

      quill.on(Quill.events.TEXT_CHANGE, (delta, oldDelta, source) => {
        onTextChangeRef.current?.(quill.root.innerHTML, delta, source, quill);
      });

      return () => {
        ref.current = null;
        container.innerHTML = "";
      };
    }, [modules, formats, placeholder, readOnly]);

    useEffect(() => {
      ref.current?.enable(!readOnly);
    }, [ref, readOnly]);

    return <div ref={containerRef}></div>;
  },
);
