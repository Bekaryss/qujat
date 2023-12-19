import React, { useEffect, useState, useCallback } from "react";
import {
  Box,
  Divider,
  TextField,
  FormControl,
  MenuItem,
  InputLabel,
  Select,
  Button,
} from "@mui/material";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import { v4 as uuidv4 } from "uuid";
import { useLocation } from "react-router-dom";

import { fetchCategory } from "../../redux/slices/categorySlice";
import { fetchSubCategories } from "../../redux/slices/subCategoriesSlice";
import { setDocumentId } from "../../redux/slices/documentsSlice";

import instance from "../../axios/axios";

import MultiplySelectSubCategory from "../../components/MultiplySelectSubCategory/MultiplySelectSubCategory";
import UpLoadFile from "../AddNewDocument/UpLoadFile/UpLoadFile";
import UpLoadFilePDF from "../AddNewDocument/UpLoadFile/UpLoadFilePDF";

const EditDocument = () => {
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const { categories, isLoading } = useSelector((state) => state.categories);
  const isLoadingCategories = isLoading === true;
  const { subCategories } = useSelector((state) => state.subCategories);
  const { documentId } = useSelector((state) => state.documents);

  const [selectedCategory, setSelectedCategory] = useState("");
  const [selectedCategoryList, setSelectedCategoryList] = useState([]);
  const [selectedValues, setSelectedValues] = useState([]);
  const [showSelectedCategoryList, setShowSelectedCategoryList] = useState([]);
  const [additionalSelects, setAdditionalSelects] = useState([]);

  const [uploadedFileId, setUploadedFileId] = useState(null);
  const [uploadedPdf, setUploadPdf] = useState(null);

  const dictonary = {};

  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
  } = useForm({
    defaultValues: {
      nameKz: "",
      nameRu: "",
      parentSubcategoryIds: [],
      sourceContentBlobId: 0,
      filledContentBlobId: null,
    },
    mode: "onBlur",
  });

  const handleAddCategory = useCallback(() => {
    const newSelectId = uuidv4();
    setAdditionalSelects((prevSelects) => [...prevSelects, newSelectId]);
  }, []);

  const handleChangeSelectSubCategory = useCallback(
    (value, selectId) => {
      setSelectedValues((prev) => ({ ...prev, [selectId]: value }));

      const id = uuidv4();
      const selectedCategoryObj = {
        id: id,
        name: selectedCategory,
        subCategories: value.map((item) => item),
      };

      const existingIndex = selectedCategoryList.findIndex(
        (obj) => obj.name === selectedCategory
      );

      // console.log("selectedCategory", selectedCategory);

      if (existingIndex !== -1) {
        // Object with the same id already exists, update subCategories
        const updatedObject = {
          ...selectedCategoryList[existingIndex],
          subCategories: [...selectedCategoryObj.subCategories],
        };

        // Update the object in the array
        selectedCategoryList[existingIndex] = updatedObject;
      } else {
        // Object with the same id does not exist, add a new object
        selectedCategoryList.push(selectedCategoryObj);
      }

      setShowSelectedCategoryList(selectedCategoryList);
    },
    [selectedCategory, selectedCategoryList]
  );

  const handleCategoryChange = useCallback((event, selectId) => {
    const value = event.target.value;
    dictonary[value] = selectId;
    setSelectedCategory((prev) => ({ ...prev, [selectId]: value }));
  }, []);

  const getSubCategory = (id) => {
    dispatch(fetchSubCategories(id));
  };

  const handleFileUpload = useCallback((id) => {
    console.log(id);
    // Обновляем contentBlobIds после успешной загрузки файла
    setUploadedFileId(id);
  }, []);

  const handleUploadPDF = useCallback((id) => {
    setUploadPdf(id);
  }, []);

  const onSubmit = async (data) => {
    const { fileType, category, ...formData } = data;
    formData.sourceContentBlobId = uploadedFileId; // Обновляем contentBlobIds
    formData.filledContentBlobId = uploadedPdf;
    console.log(formData);
    try {
      const response = await instance.post("/api/1/documents", formData, {
        headers: { accept: "*/*" },
      });

      if (response.status === 200) {
        navigate("/pattern");
      }
    } catch (error) {
      console.error("Произошла ошибка:", error);
    }
  };

  useEffect(() => {
    const getCurrentDocument = async (id) => {
      try {
        const response = await instance.get(`/api/1/documents/${id}`);

        if (response.status === 200) {
          setValue("nameKz", response.data.responseData.nameKz);
          setValue("nameRu", response.data.responseData.nameRu);
          setSelectedValues(response.data.responseData?.parentSubcategories);
          setUploadedFileId(response.data.responseData?.sourceContentBlob.uri);

          response.data.responseData.parentSubcategories.forEach((item) => {
            if (dictonary[item.parentCategory.nameKz] === undefined) {
              const newSelectId = uuidv4();
              dictonary[item.parentCategory.nameKz] = newSelectId;
            }

            if (dictonary[item.parentCategory.nameKz] !== undefined) {
              const selectedKey = dictonary[item.parentCategory.nameKz];
              const exist = additionalSelects.find(
                (item) => item === selectedKey
              );

              if (exist === undefined) {
                console.log("not exist");
                if (additionalSelects.length > 0) {
                  setAdditionalSelects((prevSelects) => [
                    ...prevSelects,
                    selectedKey,
                  ]);
                } else {
                  setAdditionalSelects(() => [selectedKey]);
                }

                setSelectedCategory((prev) => ({
                  ...prev,
                  [selectedKey]: item.parentCategory.nameKz,
                }));

                getSubCategory(item.parentCategoryId);

                // const existingIndex = selectedCategoryList.findIndex(
                //   (obj) => obj.name === selectedCategory
                // );

                console.log("selectedCategoryList", selectedKey);
                const existingIndex = selectedCategoryList.findIndex(
                  (obj) => obj.name === selectedCategory
                );

                let arr = [item];

                if (selectedCategoryList.length > 0) {
                  arr = [
                    ...selectedCategoryList[existingIndex]?.subCategories,
                    item,
                  ];
                }

                handleChangeSelectSubCategory(arr, selectedKey);
              } else {
                console.log(exist);
              }
            }
          });
        }
      } catch (err) {
        console.log(err);
      }
    };

    const pathname = location.pathname;
    const parts = pathname.split("/");
    const id = parseInt(parts[parts.length - 1], 10);
    setDocumentId(id);

    dispatch(fetchCategory());
    getCurrentDocument(id);
  }, [dispatch, documentId, setValue]);

  useEffect(() => {
    const parentSubcategoryIds = Object.values(selectedValues)
      .flat()
      .map((item) => item.id);

    setValue("parentSubcategoryIds", parentSubcategoryIds);
  }, [selectedValues, showSelectedCategoryList, setValue]);

  return (
    <div className="add_new_document_container">
      <h1>Құжат өзгерту</h1>
      <Box className="new_document_wrapper">
        <p>Құжатты өзгерту үшін деректерді енгізіңіз</p>
        <Divider
          sx={{
            display: "flex",
            width: "100%",
            height: "2px",
            background: "var(--gray-800)",
          }}
        />
        <Box sx={{ width: "80%", marginTop: 3 }}>
          <h3 style={{ marginBottom: "12px" }}>Алдымен файлды жүктеп алыңыз</h3>
          <UpLoadFile
            uploadedFileId={uploadedFileId}
            onFileUpload={handleFileUpload}
          />
        </Box>
        <Box sx={{ width: "80%", marginTop: 2 }}>
          <h3 style={{ marginBottom: "12px" }}>PDF файл</h3>
          <UpLoadFilePDF handleUploadPDF={handleUploadPDF} />
        </Box>
        <form onSubmit={handleSubmit(onSubmit)}>
          <Box sx={{ width: "100%", marginTop: 1 }}>
            <span className="main_text">
              Құжат үлгісінің қазақша атын енгізіңіз
            </span>
            <TextField
              size="small"
              sx={{ marginTop: 1 }}
              fullWidth
              error={Boolean(errors.nameKz?.message)}
              {...register("nameKz", {
                required: "Құжат үлгісінің атын енгізіңіз",
              })}
            />
          </Box>
          <Box sx={{ width: "100%", marginTop: 2 }}>
            <span className="main_text">
              Құжат үлгісінің орысша атын енгізіңіз
            </span>
            <TextField
              size="small"
              sx={{ marginTop: 1 }}
              fullWidth
              error={Boolean(errors.nameRu?.message)}
              {...register("nameRu", {
                required: "Құжат үлгісінің орысша атын енгізіңіз",
              })}
            />
          </Box>

          <Box sx={{ display: "flex", width: "100%", flexDirection: "column" }}>
            <Box
              sx={{ display: "flex", width: "100%", flexDirection: "column" }}
            >
              {additionalSelects.map((selectId) => (
                <Box
                  key={selectId}
                  sx={{
                    display: "flex",
                    width: "100%",
                    marginTop: 1,
                    gap: "30px",
                  }}
                >
                  <Box
                    sx={{
                      width: "50%",
                      marginTop: 1,
                      display: "flex",
                      flexDirection: "column",
                    }}
                  >
                    <span className="main_text">
                      Құжат категориясын таңдаңыз
                    </span>
                    <FormControl sx={{ marginTop: 2 }} size="small">
                      <InputLabel id="category">
                        Құжат категориясын таңдаңыз
                      </InputLabel>
                      <Select
                        labelId="category"
                        label="Құжат категориясының таңдаңыз"
                        value={selectedCategory[selectId] || ""}
                        onChange={(event) => {
                          handleCategoryChange(event, selectId);
                          setValue("category", event.target.value);
                        }}
                        sx={{ width: "100%" }}
                        size="small"
                      >
                        {!isLoadingCategories &&
                          categories.map((item) => (
                            <MenuItem
                              key={item.id}
                              value={item.nameKz}
                              onClick={() => getSubCategory(item.id)}
                            >
                              {item.nameKz}
                            </MenuItem>
                          ))}
                      </Select>
                    </FormControl>
                  </Box>
                  <Box sx={{ width: "50%", marginTop: 1 }}>
                    <span className="main_text">
                      Құжатның ішкі категориясын таңдаңыз
                    </span>
                    <MultiplySelectSubCategory
                      subCategories={subCategories}
                      selectedSubCategories={(value) =>
                        handleChangeSelectSubCategory(value, selectId)
                      }
                      selectedValues={selectedValues[selectId] || []}
                      selectedCategory={selectedCategory[selectId] || ""}
                    />
                  </Box>
                </Box>
              ))}
              <Button
                variant="outlined"
                color="success"
                sx={{ marginTop: 2, width: "30%" }}
                onClick={handleAddCategory}
              >
                Добавить категорию
              </Button>
            </Box>
          </Box>
          <Divider
            sx={{
              display: "flex",
              width: "100%",
              height: "2px",
              background: "var(--gray-800)",
              marginTop: 3,
              marginBottom: 3,
            }}
          />
          <Box sx={{ marginTop: 3 }}>
            <Button
              variant="contained"
              type="submit"
              disabled={uploadedFileId === null}
            >
              өзгерту
            </Button>
          </Box>
        </form>
      </Box>
    </div>
  );
};

export default EditDocument;
