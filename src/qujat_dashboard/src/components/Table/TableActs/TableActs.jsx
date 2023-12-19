import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { Link, useLocation, useNavigate, useParams } from "react-router-dom";

import {
  Box,
  TableContainer,
  TableHead,
  TableBody,
  TableRow,
  TableCell,
  Table,
  Button,
  Paper,
  CircularProgress,
  PaginationItem,
  Tooltip,
  styled,
} from "@mui/material";

import { MdEdit } from "react-icons/md";
import { MdOutlineDeleteSweep } from "react-icons/md";

import AddActs from "../../Modals/Acts/AddActs";
import UpdateActs from "../../Modals/Acts/UpdateActs";
import DeleteAct from "../../Modals/Acts/DeleteAct";
import TablePagination from "../../Pagination/TablePagination";

import {
  fetchActs,
  setId,
  changePageIndex,
} from "../../../redux/slices/actsSlice";

const StyledLink = styled(Link)({
  textDecoration: "none",
  color: "inherit",
});

const AddButton = styled(Button)({
  backgroundColor: "var(--main-color-500)",
  color: "#fff",
  padding: 11,
  "&:hover": {
    backgroundColor: "var(--main-color-btn)",
  },
  "@media (max-width: 475px)": {
    width: "100%",
    marginTop: 8,
  },
});

const TableActs = () => {
  const dispatch = useDispatch();
  const location = useLocation();
  const navigate = useNavigate();
  // const { page } = useParams();
  // const currentPage = parseInt(page, 10) || 0;

  const [modalOpenAct, setModalOpenAct] = useState(false);
  const [modalUpdateAct, setModalUpdateAct] = useState(false);
  const [modalDeleteAct, setModalDeleteAct] = useState(false);
  const [dataLength, setDataLength] = useState(); //слежение о изменение данных в таблице

  const { acts, pageIndex, isLoading } = useSelector((state) => state.acts);
  const isLoadingActs = isLoading === true; // получил ли я данные

  const pageSize = acts?.pageSize;
  const totalSize = acts?.totalSize;

  const page = 0;

  //HANDLER OPEN CREATE ACTS
  const handleOpenModal = () => {
    setModalOpenAct(true);
  };
  const handleCloseModal = () => {
    setModalOpenAct(false);
    setModalUpdateAct(false);
  };

  //HANDLER UPDATE ACTS
  const handleOpenUpdateModal = (id) => {
    dispatch(setId(id));
    setModalUpdateAct(true);
  };

  const UpdateData = () => {
    getBypage();
  };

  //HANDLER DELETE ACTS
  const handleOpenDeleteAct = (id) => {
    dispatch(setId(id));
    setModalDeleteAct(true);
  };

  const handleCloseDeleteAct = () => {
    setModalDeleteAct(false);
    UpdateData();
  };

  const handleChangePage = (_, page) => {
    if (page === 0) {
      page += 1;
    }

    dispatch(changePageIndex(page - 1));
    dispatch(fetchActs(page - 1));
    navigate(`${location.pathname}?page=${page}`);
  };

  const getBypage = () => {
    if (location.pathname === undefined) {
      dispatch(fetchActs());
    } else {
      const searchParams = new URLSearchParams(location.search);
      let page = (parseInt(searchParams.get("page")) || 1) - 1;
      dispatch(fetchActs(page));
      dispatch(changePageIndex(page));
    }
  };

  useEffect(() => {
    getBypage();
    // const searchParams = new URLSearchParams(location.search);
    // const page = parseInt(searchParams.get("page")) || 1;
    // dispatch(changePageIndex(page - 1));
  }, [dataLength]);

  return (
    <Box className="table">
      <div className="table_head">
        <AddButton onClick={handleOpenModal}>
          <span className="btn_text">Қосу</span>
        </AddButton>
      </div>
      <AddActs
        modalOpenAct={modalOpenAct}
        handleCloseModal={handleCloseModal}
        UpdateData={UpdateData}
      />
      <TableContainer
        component={Paper}
        sx={{ height: `${10 * 72}px`, overflowY: "hidden", boxShadow: "none" }}
      >
        {isLoadingActs ? (
          <TableRow>
            <TableCell colSpan={4}>
              <Box
                sx={{
                  display: "flex",
                  width: "100%",
                  alignItems: "center",
                  justifyContent: "center",
                  position: "absolute",
                  left: "2%",
                  top: "45%",
                }}
              >
                <CircularProgress size={45} />
              </Box>
            </TableCell>
          </TableRow>
        ) : (
          <Table aria-label="simple table">
            <TableHead className="table_content_head">
              <TableRow sx={{ alignItems: "center" }}>
                <TableCell sx={{ textAlign: "center" }}>
                  <h2>#</h2>
                </TableCell>
                <TableCell>
                  <h2>Нормативтік құқықтық актінің қазакша аты</h2>
                </TableCell>
                <TableCell>
                  <h2>Сілтеме</h2>
                </TableCell>
                <TableCell>
                  <h2>Әрекет</h2>
                </TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {acts?.responseData?.map((row) => (
                <TableRow
                  key={row.id}
                  sx={{
                    cursor: "pointer",
                  }}
                  className="table_item"
                >
                  <TableCell>{row.id}</TableCell>
                  <TableCell>{row.nameKz}</TableCell>
                  <TableCell>
                    <Tooltip placement="top" title={row.uri} arrow>
                      <Link
                        style={{ color: "var(--main-color-btn)" }}
                        to={row.uri}
                        target="_blank"
                        rel="noopener noreferrer"
                      >
                        Сілтеме
                      </Link>
                    </Tooltip>
                  </TableCell>
                  <TableCell>
                    <Box className="table_action">
                      <Box
                        sx={{
                          display: "flex",
                          alignItems: "center",
                          justifyContent: "center",
                          border: "1px solid var(--black)",
                          borderRadius: "8px",
                          padding: 0.5,
                          cursor: "pointer",
                          marginRight: 2,
                          transition: "var(--tran-bg-03)",
                          "&:hover": {
                            background: "var(--main-color-btn)",
                          },
                        }}
                        onClick={() => {
                          handleOpenUpdateModal(row.id);
                        }}
                      >
                        <MdEdit style={{ fontSize: "20px" }} />
                      </Box>

                      <Box
                        sx={{
                          display: "flex",
                          alignItems: "center",
                          justifyContent: "center",
                          border: "1px solid var(--black)",
                          borderRadius: "8px",
                          padding: 0.5,
                          cursor: "pointer",
                          marginRight: 2,
                          transition: "var(--tran-bg-03)",
                          "&:hover": {
                            background: "var(--main-color-btn)",
                          },
                        }}
                        onClick={() => {
                          handleOpenDeleteAct(row.id);
                        }}
                      >
                        <MdOutlineDeleteSweep
                          style={{
                            fontSize: "20px",
                            color: "var(--main-red)",
                          }}
                        />
                      </Box>
                    </Box>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        )}
      </TableContainer>
      <Box
        spacing={2}
        sx={{
          width: "100%",
          display: "flex",
          alignContent: "center",
          justifyContent: "center",
          padding: 3,
        }}
      >
        <TablePagination
          totalSize={totalSize}
          pageIndex={pageIndex}
          pageSize={pageSize}
          handleChangePage={handleChangePage}
          renderItem={(item) => (
            <PaginationItem
              component={StyledLink}
              to={`${location.pathname}?page=${item.page}`}
              {...item}
            />
          )}
        />
      </Box>
      <UpdateActs
        modalUpdateAct={modalUpdateAct}
        handleCloseUpdateModal={handleCloseModal}
        UpdateData={UpdateData}
      />
      <DeleteAct
        deleteItem={modalDeleteAct}
        handleCloseDeleteModal={handleCloseDeleteAct}
        UpdateData={UpdateData}
      />
    </Box>
  );
};

export default TableActs;
