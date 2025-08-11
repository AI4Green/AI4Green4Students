import { useToast } from "@chakra-ui/react";
import merge from "lodash-es/merge";
import { useEffect } from "react";
import { useLocation } from "react-router-dom";

export const useLocationStateToast = (defaults = {}) => {
  const toast = useToast();
  const { state } = useLocation();
  useEffect(() => {
    if (state?.toast) toast(merge({}, defaults, state.toast));
  }, [defaults, state, toast]);
};
