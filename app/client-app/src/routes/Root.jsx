import { Routes, Route, useNavigate } from "react-router-dom";
import { useEffect } from "react";
import { DefaultLayout } from "layouts/DefaultLayout";
import { ProtectedRoutes } from "layouts/ProtectedRoutes";
import { useUser } from "contexts/User";
import { ContentPage } from "pages/ContentPage";
import { NotFound } from "pages/error/NotFound";
import { UserHome } from "pages/UserHome";
import { Account } from "./Account";
import { Admin } from "./Admin";
import { Project } from "./Project";
import GreenMetrics from "pages/GreenMetrics";

const IndexRedirect = () => {
  const { user } = useUser();
  const navigate = useNavigate();
  useEffect(() => {
    const targetPath = user ? "/home" : "/about";
    navigate(targetPath, { replace: true });
  }, [user]);
  return null;
};

export const Root = () => {
  return (
    <Routes>
      <Route path="/" element={<DefaultLayout />}>
        <Route index element={<IndexRedirect />} />

        <Route path="home" element={<ProtectedRoutes />}>
          <Route index element={<UserHome />} />
        </Route>

        <Route path="greenchemistry" element={<ProtectedRoutes />}>
          <Route
            index
            element={<ContentPage contentKey={"greenchemistry"} />}
          />
        </Route>
        <Route path="metrics" element={<ProtectedRoutes />}>
          <Route index element={<GreenMetrics />} />
        </Route>

        <Route path="about" element={<ContentPage contentKey={"about"} />} />

        <Route path="project/*" element={<Project />} />

        <Route path="account/*" element={<Account />} />

        <Route path="admin/*" element={<Admin />} />

        <Route path="*" element={<NotFound />} />
      </Route>
    </Routes>
  );
};
