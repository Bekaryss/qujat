import { Routes, Route, Navigate } from "react-router-dom";
import { useSelector } from "react-redux";

import Home from "../pages/Home/Home";
import Statistics from "../pages/Statistics/Statistics";
import Acts from "../pages/Acts/Acts";
import Pattern from "../pages/Pattern/Pattern";
import SubCategoriesPage from "../pages/SubCategoriesPage/SubCategoriesPage";
import Login from "../pages/Login/Login";
import AddNewDocument from "../pages/AddNewDocument/AddNewDocument";
import AddNewAdmmin from "../pages/AddNewAdmin/AddNewAdmmin";
import EditDocument from "../pages/EditDocument/EditDocument";

const Router = () => {
  const { pageIndex } = useSelector((state) => state.acts);
  const { documentId } = useSelector((state) => state.documents);
  return (
    <>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/add-admin" element={<AddNewAdmmin />} />
        <Route
          path="/subcategories/:categoryId"
          element={<SubCategoriesPage />}
        />
        <Route path="/acts" element={<Acts />} />
        <Route path="/pattern" element={<Pattern />} />
        <Route path="/pattern/new-document" element={<AddNewDocument />} />
        <Route
          path={`/edit-document/:${documentId}`}
          element={<EditDocument />}
        />
        <Route path="/statistics" element={<Statistics />} />
        <Route path="/sign-in" element={<Login />} />
      </Routes>
    </>
  );
};

export default Router;
