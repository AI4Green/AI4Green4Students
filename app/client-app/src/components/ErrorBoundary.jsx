import { Container, Text } from "@chakra-ui/react";
import { cloneElement, Component } from "react";
import { useLocation } from "react-router-dom";
import { TitledAlert } from "./TitledAlert";
import { isEqual } from "lodash-es";
import { useTranslation } from "react-i18next";
import { HTTPError } from "ky";

const DefaultFallback = ({ tKey, error }) => {
  const { t } = useTranslation();
  let message = t(tKey ?? "feedback.error");

  if (error instanceof HTTPError) {
    switch (error.response.status) {
      case 400:
        message = t("feedback.error_400");
        break;
      case 401:
        message = t("feeback.error_401");
        break;
      case 403:
        message = t("feedback.error_403");
        break;
      case 404:
        message = t("feedback.error_404");
        break;
      case 500:
      default:
        message = t("feedback.error");
        break;
    }
  }

  return (
    <Container my={16}>
      <TitledAlert status="error" title={t("feedback.error_title")}>
        <Text>{message}</Text>
      </TitledAlert>
    </Container>
  );
};
const LocationProvider = ({ children }) => {
  const location = useLocation();
  return children(location);
};

class LocationAwareErrorBoundary extends Component {
  state = { hasError: false, error: null };
  static getDerivedStateFromError(error) {
    return {
      hasError: true,
      error,
    };
  }

  componentDidCatch(error, info) {
    console.error("ErrorBoundary caught an error", error, info);
  }

  componentDidUpdate(prevProps) {
    if (this.props.resetOnPropChanges && !isEqual(prevProps, this.props)) {
      this.setState({
        hasError: false,
      });
    }
    if (prevProps.location.key !== this.props.location.key) {
      this.setState({
        hasError: false,
      });
    }
  }

  render() {
    if (this.state.hasError) {
      return cloneElement(this.props.fallback ?? <DefaultFallback />, {
        error: this.state.error,
      });
    }
    return this.props.children;
  }
}

export class ErrorBoundary extends Component {
  render() {
    return (
      <LocationProvider>
        {(location) => (
          <LocationAwareErrorBoundary location={location} {...this.props} />
        )}
      </LocationProvider>
    );
  }
}
